CREATE DATABASE IF NOT EXISTS bebesote;
USE bebesote;

CREATE TABLE jugador (
    idJugador INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL
);

CREATE TABLE partida (
    idPartida INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    fecha_inicio DATETIME,
    id_ganador INT,
    fecha_fin DATETIME
);

CREATE TABLE jugador_partida (
    idJugador INT NOT NULL,
    idPartida INT NOT NULL,
    resultado VARCHAR(50),
    PRIMARY KEY (idJugador, idPartida)
);

CREATE TABLE log_movimiento (
    id_movimiento INT PRIMARY KEY,
    id_Partida INT NOT NULL,
    id_Jugador INT NOT NULL,
    numero_turno INT,
    accion VARCHAR(50),
    descripcion VARCHAR(255),
    `timestamp` DATETIME
);

alter table partida
add constraint fk_partida_ganador
foreign key (id_ganador)
references jugador (idJugador)
on delete set null
on update cascade;

alter table jugador_partida
add constraint fk_jugador_jp
foreign key (idJugador)
references jugador (idJugador)
on delete cascade
on update cascade,
add constraint fk_partida_jp
foreign key (idPartida)
references partida (idPartida)
on delete cascade
on update cascade;

alter table log_movimiento
add constraint fk_logmov_jugadorpartida
foreign key (id_Jugador, id_Partida)
references jugador_partida (idJugador, idPartida)
on delete cascade
on update cascade;

ALTER TABLE log_movimiento
MODIFY `timestamp` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;

DESCRIBE log_movimiento;

ALTER TABLE log_movimiento
MODIFY id_movimiento INT NOT NULL AUTO_INCREMENT;

INSERT INTO jugador (nombre) VALUES ('Carlos');

INSERT INTO partida (fecha_inicio, id_ganador, fecha_fin)
VALUES (NOW(), 1, NOW());

INSERT INTO jugador_partida (idJugador, idPartida, resultado)
VALUES (1, 1, 'ganador');

INSERT INTO log_movimiento (id_Partida, id_Jugador, numero_turno, accion, descripcion)
VALUES (1, 1, 1, 'jugar_carta', 'Rojo 7');

DESCRIBE partida;

#Borra los datos que habia puesto como verificación
DELETE FROM log_movimiento;
DELETE FROM jugador_partida;
DELETE FROM partida;
DELETE FROM jugador;
ALTER TABLE jugador AUTO_INCREMENT = 1;
ALTER TABLE partida AUTO_INCREMENT = 1;
ALTER TABLE log_movimiento AUTO_INCREMENT = 1;

SELECT * FROM jugador;

SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE log_movimiento;
TRUNCATE TABLE jugador_partida;
TRUNCATE TABLE partida;
TRUNCATE TABLE jugador;
SET FOREIGN_KEY_CHECKS = 1;

ALTER TABLE jugador ADD UNIQUE (nombre);