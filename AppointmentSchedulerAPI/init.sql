/* ---------------------------------------------------- */
/*  Generated by Enterprise Architect Version 15.0 		*/
/*  Created On : 20-Dec-2024 2:07:10 PM 				*/
/*  DBMS       : PostgreSQL 						*/
/* ---------------------------------------------------- */

/* Drop Sequences for Autonumber Columns */

DROP SEQUENCE IF EXISTS appointment_id_seq
;

DROP SEQUENCE IF EXISTS assistant_id_seq
;

DROP SEQUENCE IF EXISTS availabilitytimeslot_id_seq
;

DROP SEQUENCE IF EXISTS client_id_seq
;

DROP SEQUENCE IF EXISTS service_id_seq
;

DROP SEQUENCE IF EXISTS useraccount_id_seq
;

/* Drop Tables */

DROP TABLE IF EXISTS "Appointment" CASCADE
;

DROP TABLE IF EXISTS "AppointmentService" CASCADE
;

DROP TABLE IF EXISTS "Assistant" CASCADE
;

DROP TABLE IF EXISTS "AssistantService" CASCADE
;

DROP TABLE IF EXISTS "AvailabilityTimeSlot" CASCADE
;

DROP TABLE IF EXISTS "Client" CASCADE
;

DROP TABLE IF EXISTS "Service" CASCADE
;

DROP TABLE IF EXISTS "UserAccount" CASCADE
;

DROP TABLE IF EXISTS "UserInformation" CASCADE
;


CREATE TYPE public."ClientStatusType" AS ENUM ('ENABLED', 'DISABLED', 'DELETED');

CREATE TYPE public."AssistantStatusType" AS ENUM ('ENABLED', 'DISABLED', 'DELETED');

CREATE TYPE public."ServiceStatusType" AS ENUM ('ENABLED', 'DISABLED', 'DELETED');

CREATE TYPE public."AppointmentStatusType" AS ENUM ('SCHEDULED','CONFIRMED', 'CANCELED', 'FINISHED');

CREATE TYPE public."RoleType" AS ENUM ('ADMINISTRATOR', 'CLIENT', 'ASSISTANT');



/* Create Tables */

CREATE TABLE "Appointment"
(
	date timestamp with time zone NULL,
	"endTime" time without time zone NULL,
	"startTime" time without time zone NULL,
	status public."AppointmentStatusType" NULL,
	"totalCost" numeric NULL,
	uuid uuid NULL,
	id integer NOT NULL   DEFAULT NEXTVAL(('"appointment_id_seq"'::text)::regclass),
	created_at timestamp without time zone NULL   DEFAULT CURRENT_TIMESTAMP,
	id_client integer NULL,
	id_assistant integer NULL
)
;

CREATE TABLE "AppointmentService"
(
	id_service integer NULL,
	id_appointment integer NULL
)
;

CREATE TABLE "Assistant"
(
	uuid uuid NULL,
	id_user integer NOT NULL,
	id integer NOT NULL   DEFAULT NEXTVAL(('"assistant_id_seq"'::text)::regclass),
	status public."AssistantStatusType" NULL
)
;

CREATE TABLE "AssistantService"
(
	id_assistant integer NULL,
	id_service integer NULL
)
;

CREATE TABLE "AvailabilityTimeSlot"
(
	id integer NOT NULL   DEFAULT NEXTVAL(('"availabilitytimeslot_id_seq"'::text)::regclass),
	uuid uuid NULL,
	date varchar(50) NULL,
	"startTime" varchar(50) NULL,
	"endTime" varchar(50) NULL,
	created_at timestamp without time zone NULL   DEFAULT CURRENT_TIMESTAMP,
	id_assistant integer NOT NULL
)
;

CREATE TABLE "Client"
(
	uuid uuid NULL,
	id_user integer NOT NULL,
	id integer NOT NULL   DEFAULT NEXTVAL(('"client_id_seq"'::text)::regclass),
	status public."ClientStatusType" NULL
)
;

CREATE TABLE "Service"
(
	description text NULL,
	minutes integer NULL,
	name varchar(100) NULL,
	price double precision NULL,
	id integer NOT NULL   DEFAULT NEXTVAL(('"service_id_seq"'::text)::regclass),
	uuid uuid NULL,
	created_at timestamp without time zone NULL   DEFAULT CURRENT_TIMESTAMP,
	status public."ServiceStatusType" NULL
)
;

CREATE TABLE "UserAccount"
(
	email varchar(100) NULL,
	password varchar(50) NULL,
	username varchar(16) NULL,
	created_at timestamp without time zone NULL   DEFAULT CURRENT_TIMESTAMP,
	id integer NOT NULL   DEFAULT NEXTVAL(('"useraccount_id_seq"'::text)::regclass),
	role public."RoleType" NULL
)
;

CREATE TABLE "UserInformation"
(
	name varchar(100) NULL,
	"phoneNumber" varchar(15) NULL,
	filepath varchar(255) NULL,
	id_user integer NOT NULL
)
;

/* Create Primary Keys, Indexes, Uniques, Checks */

ALTER TABLE "Appointment" ADD CONSTRAINT "PK_Appointment"
	PRIMARY KEY (id)
;

CREATE INDEX "IXFK_Appointment_Assistant" ON "Appointment" (id_assistant ASC)
;

CREATE INDEX "IXFK_Appointment_Client" ON "Appointment" (id_client ASC)
;

CREATE INDEX "IXFK_AppointmentService_Appointment" ON "AppointmentService" (id_appointment ASC)
;

CREATE INDEX "IXFK_AppointmentService_Service" ON "AppointmentService" (id_service ASC)
;

ALTER TABLE "Assistant" ADD CONSTRAINT "PK_Assistant"
	PRIMARY KEY (id)
;

CREATE INDEX "IXFK_Assistant_UserAccount" ON "Assistant" (id_user ASC)
;

CREATE INDEX "IXFK_AssistantService_Assistant" ON "AssistantService" (id_assistant ASC)
;

CREATE INDEX "IXFK_AssistantService_Service" ON "AssistantService" (id_service ASC)
;

ALTER TABLE "AvailabilityTimeSlot" ADD CONSTRAINT "PK_AvailabilityTimeSlot"
	PRIMARY KEY (id)
;

CREATE INDEX "IXFK_AvailabilityTimeSlot_Assistant" ON "AvailabilityTimeSlot" (id_assistant ASC)
;

ALTER TABLE "Client" ADD CONSTRAINT "PK_Client"
	PRIMARY KEY (id)
;

CREATE INDEX "IXFK_Client_UserAccount" ON "Client" (id_user ASC)
;

ALTER TABLE "Service" ADD CONSTRAINT "PK_Service"
	PRIMARY KEY (id)
;

ALTER TABLE "UserAccount" ADD CONSTRAINT "PK_UserAccount"
	PRIMARY KEY (id)
;

CREATE INDEX "IXFK_UserInformation_UserAccount" ON "UserInformation" (id_user ASC)
;

/* Create Foreign Key Constraints */

ALTER TABLE "Appointment" ADD CONSTRAINT "FK_Appointment_Assistant"
	FOREIGN KEY (id_assistant) REFERENCES "Assistant" (id) ON DELETE Cascade ON UPDATE Cascade
;

ALTER TABLE "Appointment" ADD CONSTRAINT "FK_Appointment_Client"
	FOREIGN KEY (id_client) REFERENCES "Client" (id) ON DELETE Cascade ON UPDATE Cascade
;

ALTER TABLE "AppointmentService" ADD CONSTRAINT "FK_AppointmentService_Appointment"
	FOREIGN KEY (id_appointment) REFERENCES "Appointment" (id) ON DELETE Cascade ON UPDATE Cascade
;

ALTER TABLE "AppointmentService" ADD CONSTRAINT "FK_AppointmentService_Service"
	FOREIGN KEY (id_service) REFERENCES "Service" (id) ON DELETE Cascade ON UPDATE Cascade
;

ALTER TABLE "Assistant" ADD CONSTRAINT "FK_Assistant_UserAccount"
	FOREIGN KEY (id_user) REFERENCES "UserAccount" (id) ON DELETE Cascade ON UPDATE Cascade
;

ALTER TABLE "AssistantService" ADD CONSTRAINT "FK_AssistantService_Assistant"
	FOREIGN KEY (id_assistant) REFERENCES "Assistant" (id) ON DELETE Cascade ON UPDATE Cascade
;

ALTER TABLE "AssistantService" ADD CONSTRAINT "FK_AssistantService_Service"
	FOREIGN KEY (id_service) REFERENCES "Service" (id) ON DELETE Cascade ON UPDATE Cascade
;

ALTER TABLE "AvailabilityTimeSlot" ADD CONSTRAINT "FK_AvailabilityTimeSlot_Assistant"
	FOREIGN KEY (id_assistant) REFERENCES "Assistant" (id) ON DELETE Cascade ON UPDATE Cascade
;

ALTER TABLE "Client" ADD CONSTRAINT "FK_Client_UserAccount"
	FOREIGN KEY (id_user) REFERENCES "UserAccount" (id) ON DELETE Cascade ON UPDATE Cascade
;

ALTER TABLE "UserInformation" ADD CONSTRAINT "FK_UserInformation_UserAccount"
	FOREIGN KEY (id_user) REFERENCES "UserAccount" (id) ON DELETE Cascade ON UPDATE Cascade
;

/* Create Table Comments, Sequences for Autonumber Columns */

CREATE SEQUENCE appointment_id_seq INCREMENT 1 START 1
;

CREATE SEQUENCE assistant_id_seq INCREMENT 1 START 1
;

CREATE SEQUENCE availabilitytimeslot_id_seq INCREMENT 1 START 1
;

CREATE SEQUENCE client_id_seq INCREMENT 1 START 1
;

CREATE SEQUENCE service_id_seq INCREMENT 1 START 1
;

CREATE SEQUENCE useraccount_id_seq INCREMENT 1 START 1
;