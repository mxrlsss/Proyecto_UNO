from fastapi import FastAPI
from pydantic import BaseModel
from typing import Optional 
from fastapi import HTTPException
from contextlib import asynccontextmanager

import pymysql

def get_conexion():
    return pymysql.connect(
        host="127.0.0.1", port=3306, user="root", password="M4ng0ph*nk",
        database="bebesote", cursorclass=pymysql.cursors.DictCursor
    )

"""Crea nuestros jufadores por defecto"""
def crear_jugadores_base():
    con = get_conexion()
    try:
        with con.cursor() as cur:
            for nombre in ("Iván", "Alexis"):
                cur.execute("INSERT IGNORE INTO jugador (nombre) VALUES (%s)", (nombre,))
        con.commit()
    finally:
        con.close()

@asynccontextmanager
async def lifespan(app: FastAPI):
    # Esto corre al ARRANCAR la API
    crear_jugadores_base()
    yield

app = FastAPI(lifespan=lifespan)

"""Clases para crear jugadores, partidas y movimientos"""
class JugadorCrear(BaseModel):
    nombre: str

class IniciarPartida(BaseModel):
    id_jugador1: int
    id_jugador2: int

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

<<<<<<< HEAD
def get_conexion():
    return pymysql.connect(
        host="127.0.0.1", port=3306, user="root", password="Morales07",
        database="bebesote", cursorclass=pymysql.cursors.DictCursor
    )
=======
"""Mostrar Partidas y Movimientos, obteniendo el JSON de la base de datos"""
>>>>>>> 533cef6d9d4c12c3db91b101b9824b5d3e0dc9df

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

"""Actualizar Partidas"""

"""Iniciar una partida, insertando los jugadores y la fecha de inicio en la base de datos"""
@app.post("/partidas")
async def iniciar_partida(datos: IniciarPartida):
    if datos.id_jugador1 == datos.id_jugador2:
        raise HTTPException(status_code=400, detail="Los jugadores deben ser distintos")

    con = get_conexion()
    try:
        with con.cursor() as cur:
            cur.execute("INSERT INTO Partida (fecha_inicio) VALUES (NOW())")
            id_partida = cur.lastrowid

            for id_jugador in (datos.id_jugador1, datos.id_jugador2):
                cur.execute(
                    "INSERT INTO Jugador_Partida (idJugador, idPartida, resultado) "
                    "VALUES (%s, %s, %s)",
                    (id_jugador, id_partida, "en_curso")
                )
        con.commit()
        return {"idPartida": id_partida}
    except pymysql.err.IntegrityError:
        con.rollback()
        raise HTTPException(status_code=400, detail="Alguno de los jugadores no existe")
    finally:
        con.close()

"""Registrar un movimiento, insertando el movimiento en la base de datos"""

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
        con.commit()
        return {"idMovimiento": cur.lastrowid}
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