from fastapi import FastAPI
from pydantic import BaseModel
from typing import List
from fastapi import HTTPException
from contextlib import asynccontextmanager

import pymysql

def get_conexion():
    return pymysql.connect(
        host="127.0.0.1", port=3306, user="root", password="M4ng0ph*nk", #Morales07 contraseña base datos Alexis's PC, M4ng0ph*nk Ivan's PC
        database="bebesote", cursorclass=pymysql.cursors.DictCursor
    )

MIN_JUGADORES = 2
MAX_JUGADORES = 4

app = FastAPI()

class JugadorCrear(BaseModel):
    nombre: str

class IniciarPartida(BaseModel):
    ids_jugadores: List[int] 

class PartidaCrear(BaseModel):
    fecha_inicio: str   

class JugadorPartidaCrear(BaseModel):
    id_jugador: int

class Movimiento(BaseModel):
    id_jugador: int
    numero_turno: int
    accion: str
    descripcion: str

class PartidaFinalizar(BaseModel):
    id_ganador: int


"""Mostrar Partidas y Movimientos, obteniendo el JSON de la base de datos"""

@app.get("/partidas")
async def listar_partidas():
    con = get_conexion()
    try:
        with con.cursor() as cur:
            cur.execute("""
                SELECT p.idPartida, p.fecha_inicio, p.fecha_fin,
                       j.nombre AS ganador
                FROM partida p
                LEFT JOIN jugador j ON p.id_ganador = j.idJugador
                ORDER BY p.fecha_inicio DESC
            """)
            partidas = cur.fetchall()
            for partida in partidas:
                cur.execute("""
                    SELECT j.idJugador, j.nombre, jp.resultado
                    FROM Jugador_Partida jp
                    JOIN jugador j ON jp.idJugador = j.idJugador
                    WHERE jp.idPartida = %s
                """, (partida['idPartida'],))
                partida['jugadores'] = cur.fetchall()
            return partidas
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
                JOIN jugador j ON jp.idJugador = j.idJugador
                WHERE jp.idPartida = %s
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
                       m.descripcion, m.`timestamp` as tiempo_jugada
                FROM log_movimiento m
                JOIN jugador j ON m.id_Jugador = j.idJugador
                WHERE m.id_Partida = %s
                ORDER BY m.numero_turno
            """, (idPartida,))
            return cur.fetchall()
    finally:
        con.close()

@app.get("/jugadores")
async def listar_jugadores():
    con = get_conexion()
    try:
        with con.cursor() as cur:
            cur.execute("SELECT idJugador, nombre FROM jugador ORDER BY nombre")
            return cur.fetchall()
    finally: 
        con.close()

@app.get("/partidas/{idPartida}")
async def detalle_partida(idPartida: int):
    con = get_conexion()
    try: 
        with con.cursor() as cur:
            cur.execute("""
                SELECT p.idPartida, p.fecha_inicio, p.fecha_fin,
                       p.id_ganador, j.nombre as ganador
                       FROM partida p
                       LEFT JOIN jugador j ON p.id_ganador = j.idJugador
                       WHERE p.idPartida = %s)
                       """, (idPartida,))
            partida = cur.fetchone()
            if partida is None:
                raise HTTPException(status_code=404, detail="Partida no encontrada")

            cur.execute("""
                SELECT j.idJugador, j.nombre, jp.resultado
                FROM Jugador_partida jp
                JOIN jugador j ON jp.idJugador = j.idJugador
                WHERE jp.idPartida = %s
            """, (idPartida,))
            partida["jugadores"] = cur.fetchall()

            cur.execute("""
                SELECT m.idMovimiento, m.numero_turno, m.id_Jugador,
                       j.nombre AS jugador, m.accion, m.'timestamp' AS tiempo_jugada
                       FROM log_movimiento
                       JOIN jugador j ON m.id_Jugador = j.idJugador
                       WHERE m.id_Partida = %s
                       ORDER BY m.numero_turno
                       """, (idPartida,))
            partida["movimientos"] = cur.fetchall()

            return partida
    finally:
        con.close()
            

"""Actualizar Partidas"""

"""Iniciar una partida, insertando los jugadores y la fecha de inicio en la base de datos"""

@app.post("/jugadores")
async def crear_jugador(datos:JugadorCrear):
    nombre = datos.nombre.strip()
    if not nombre:
        raise HTTPException(status_code=400, detail="El nombre del jugador no puede estar vacío")
    con = get_conexion()
    try:
        with con.cursor() as cur:
            cur.execute("INSERT INTO jugador (nombre) VALUES (%s)", (nombre,))
            id_jugador = cur.lastrowid
        con.commit()
        return {"idJugador": id_jugador, "nombre": nombre}
    finally:
        con.close()

@app.post("/partidas")
async def iniciar_partida(datos: IniciarPartida):
    """Crea la partida e inscribe de 2 a 4 jugadores."""
    ids = datos.ids_jugadores
 
    if len(ids) != len(set(ids)):
        raise HTTPException(status_code=400, detail="Hay jugadores repetidos")
    if not (MIN_JUGADORES <= len(ids) <= MAX_JUGADORES):
        raise HTTPException(
            status_code=400,
            detail=f"Una partida necesita entre {MIN_JUGADORES} y {MAX_JUGADORES} jugadores"
        )
 
    con = get_conexion()
    try:
        with con.cursor() as cur:
            # Verificar que todos los jugadores existan
            formato = ",".join(["%s"] * len(ids))
            cur.execute(f"SELECT idJugador FROM jugador WHERE idJugador IN ({formato})", ids)
            if len(cur.fetchall()) != len(ids):
                raise HTTPException(status_code=400, detail="Alguno de los jugadores no existe")
 
            cur.execute("INSERT INTO Partida (fecha_inicio) VALUES (NOW())")
            id_partida = cur.lastrowid
 
            for id_jugador in ids:
                cur.execute(
                    "INSERT INTO Jugador_Partida (idJugador, idPartida, resultado) "
                    "VALUES (%s, %s, %s)",
                    (id_jugador, id_partida, "en_curso")
                )
        con.commit()
        return {"idPartida": id_partida, "jugadores": ids}
    except pymysql.err.IntegrityError:
        con.rollback()
        raise HTTPException(status_code=400, detail="No se pudo inscribir a los jugadores")
    finally:
        con.close()

@app.post("/partidas/{idPartida}/movimientos")
async def registrar_movimiento(idPartida: int, movimiento: Movimiento):
    con = get_conexion()
    try:
        with con.cursor() as cur:
            cur.execute("""
                INSERT INTO log_movimiento (id_Partida, id_Jugador, numero_turno, accion, descripcion)
                VALUES (%s, %s, %s, %s, %s)
            """, (idPartida, movimiento.id_jugador, movimiento.numero_turno,
                  movimiento.accion, movimiento.descripcion))
            id_now = cur.lastrowid
        con.commit()
        return {"idMovimiento": id_now}
    except pymysql.err.IntegrityError:
        con.rollback()
        raise HTTPException(
            status_code=400,
            detail="El jugador no está inscrito en esa partida"
        )
    finally:
        con.close()

"""Finalizar la partida"""
@app.put("/partidas/{id_partida}/finalizar")
async def finalizar_partida(id_partida: int, datos: PartidaFinalizar):
    con = get_conexion()
    try:
        with con.cursor() as cur:
            cur.execute(
                "UPDATE Partida SET id_ganador = %s, fecha_fin = NOW() "
                "WHERE idPartida = %s",
                (datos.id_ganador, id_partida)
            )

            if cur.fetchone() is None:
                raise HTTPException(
                    status_code=400,
                    detail="El ganador no está inscrito en esa partida"
                )
            
            cur.execute(
                "UPDATE Partida SET id_ganador = %s, fecha_fin = NOW() "
                "WHERE idPartida = %s",
                (datos.id_ganador, id_partida)
            )
            cur.execute(
                "UPDATE Jugador_Partida "
                "SET resultado = IF(idJugador = %s, 'ganador', 'perdedor') "
                "WHERE idPartida = %s",
                (datos.id_ganador, id_partida)
            )
        con.commit()
        return {"ok": True}
    finally:
        con.close()