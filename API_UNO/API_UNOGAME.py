from typing import List
import pymysql
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel

app = FastAPI()

MIN_JUGADORES = 2
MAX_JUGADORES = 4


def get_conexion():
    return pymysql.connect(
        host="127.0.0.1", port=3306, user="root", password="M4ng0ph*nk",  # Morales07 -> Alexis, M4ng0ph*nk -> Iván
        database="bebesote", cursorclass=pymysql.cursors.DictCursor
    )

#Clases pydantic
class CrearPartida(BaseModel):
    jugadores: List[str]          # nombres de los jugadores (de 2 a 4)

class Movimiento(BaseModel):
    id_jugador: int
    numero_turno: int
    accion: str
    descripcion: str

class PartidaFinalizar(BaseModel):
    id_ganador: int


#gettser
@app.get("/partidas")
async def listar_partidas():
    con = get_conexion()
    try:
        with con.cursor() as cur:
            cur.execute("""
                SELECT p.idPartida, p.fecha_inicio, p.fecha_fin,
                       g.nombre AS ganador,
                       GROUP_CONCAT(j.nombre ORDER BY j.nombre SEPARATOR ', ') AS jugadores,
                       COUNT(jp.idJugador) AS num_jugadores
                FROM Partida p
                LEFT JOIN Jugador g          ON p.id_ganador = g.idJugador
                LEFT JOIN Jugador_Partida jp ON jp.idPartida = p.idPartida
                LEFT JOIN Jugador j          ON j.idJugador  = jp.idJugador
                GROUP BY p.idPartida, p.fecha_inicio, p.fecha_fin, g.nombre
                ORDER BY p.fecha_inicio DESC, p.idPartida DESC
            """)
            return cur.fetchall()
    finally:
        con.close()

@app.get("/partidas/{idPartida}/jugadores")
async def listar_jugadores_partida(idPartida: int):
    con = get_conexion()
    try:
        with con.cursor() as cur:
            cur.execute("""
                SELECT j.idJugador, j.nombre, jp.resultado
                FROM Jugador_Partida jp
                JOIN Jugador j ON jp.idJugador = j.idJugador
                WHERE jp.idPartida = %s
                ORDER BY j.nombre
            """, (idPartida,))
            return cur.fetchall()
    finally:
        con.close()

@app.get("/partidas/{idPartida}/movimientos")
async def listar_movimientos(idPartida: int):
    con = get_conexion()
    try:
        with con.cursor() as cur:
            cur.execute("""
                SELECT m.numero_turno, j.nombre AS jugador, m.accion,
                       m.descripcion, m.`timestamp` AS tiempo_jugada
                FROM Log_Movimiento m
                JOIN Jugador j ON m.id_Jugador = j.idJugador
                WHERE m.id_Partida = %s
                ORDER BY m.numero_turno, m.`timestamp`
            """, (idPartida,))
            return cur.fetchall()
    finally:
        con.close()


#post o updates

@app.post("/partidas")
async def crear_partida(datos: CrearPartida):
    """Registra una partida nueva con sus jugadores. Los jugadores que no existan se crean por nombre."""
    nombres = [n.strip() for n in datos.jugadores]

    if any(not n for n in nombres):
        raise HTTPException(status_code=400, detail="Todos los jugadores deben tener nombre")
    if not (MIN_JUGADORES <= len(nombres) <= MAX_JUGADORES):
        raise HTTPException(
            status_code=400,
            detail=f"Una partida necesita entre {MIN_JUGADORES} y {MAX_JUGADORES} jugadores")
    if len({n.lower() for n in nombres}) != len(nombres):
        raise HTTPException(status_code=400, detail="Hay jugadores con el mismo nombre")

    con = get_conexion()
    try:
        with con.cursor() as cur:
            jugadores = []
            for nombre in nombres:
                cur.execute("SELECT idJugador, nombre FROM Jugador WHERE nombre = %s", (nombre,))
                fila = cur.fetchone()
                if fila is None:
                    cur.execute("INSERT INTO Jugador (nombre) VALUES (%s)", (nombre,))
                    fila = {"idJugador": cur.lastrowid, "nombre": nombre}
                jugadores.append(fila)

            cur.execute("INSERT INTO Partida (fecha_inicio) VALUES (NOW())")
            id_partida = cur.lastrowid

            for j in jugadores:
                cur.execute(
                    "INSERT INTO Jugador_Partida (idJugador, idPartida, resultado) VALUES (%s, %s, %s)",
                    (j["idJugador"], id_partida, "en_curso"))
        con.commit()
        return {"idPartida": id_partida, "jugadores": jugadores}
    finally:
        con.close()

@app.post("/partidas/{idPartida}/movimientos")
async def registrar_movimiento(idPartida: int, movimiento: Movimiento):
    con = get_conexion()
    try:
        with con.cursor() as cur:
            cur.execute("""
                INSERT INTO Log_Movimiento (id_Partida, id_Jugador, numero_turno, accion, descripcion)
                VALUES (%s, %s, %s, %s, %s)
            """, (idPartida, movimiento.id_jugador, movimiento.numero_turno,
                  movimiento.accion, movimiento.descripcion))
            id_mov = cur.lastrowid
        con.commit()
        return {"idMovimiento": id_mov}
    except pymysql.err.IntegrityError:
        con.rollback()
        raise HTTPException(status_code=400, detail="El jugador no está inscrito en esa partida")
    finally:
        con.close()

@app.put("/partidas/{idPartida}/finalizar")
async def finalizar_partida(idPartida: int, datos: PartidaFinalizar):
    con = get_conexion()
    try:
        with con.cursor() as cur:
            cur.execute(
                "SELECT 1 FROM Jugador_Partida WHERE idPartida = %s AND idJugador = %s",
                (idPartida, datos.id_ganador))
            if cur.fetchone() is None:
                raise HTTPException(status_code=400, detail="El ganador no está inscrito en esa partida")

            cur.execute(
                "UPDATE Partida SET id_ganador = %s, fecha_fin = NOW() WHERE idPartida = %s",
                (datos.id_ganador, idPartida))
            cur.execute(
                "UPDATE Jugador_Partida "
                "SET resultado = IF(idJugador = %s, 'ganador', 'perdedor') "
                "WHERE idPartida = %s",
                (datos.id_ganador, idPartida))
        con.commit()
        return {"ok": True}
    finally:
        con.close()


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)