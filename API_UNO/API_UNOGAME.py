from fastapi import FastAPI
from pydantic import BaseModel
from typing import Optional 
import pymysql

app = FastAPI()

class JugadorCrear(BaseModel):
    nombre: str

class PartidaCrear(BaseModel):
    fecha_inicio: str   

class JugadorPartidaCrear(BaseModel):
    id_jugador: int

class Movimiento(BaseModel):
    id_jugador: int
    num_turno: int
    accion: str
    descripcion: str
    tiempo_jugada: float

class PartidaFinalizar(BaseModel):
    id_ganador: int
    fecha_fin: str

def get_conexion():
    return pymysql.connect(
        host="127.0.0.1", port=3306, user="root", password="Morales07",
        database="bebesote", cursorclass=pymysql.cursors.DictCursor
    )

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