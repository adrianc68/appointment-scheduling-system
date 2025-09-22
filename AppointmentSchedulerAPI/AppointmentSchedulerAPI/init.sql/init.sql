--
-- PostgreSQL database dump
--

-- Dumped from database version 17.5 (Debian 17.5-1.pgdg120+1)
-- Dumped by pg_dump version 17.5 (Debian 17.5-1.pgdg120+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: AccountStatusType; Type: TYPE; Schema: public; Owner: appdbuser
--

CREATE TYPE public."AccountStatusType" AS ENUM (
    'ENABLED',
    'DISABLED',
    'DELETED'
);


ALTER TYPE public."AccountStatusType" OWNER TO appdbuser;

--
-- Name: AppointmentNotificationCodeType; Type: TYPE; Schema: public; Owner: appdbuser
--

CREATE TYPE public."AppointmentNotificationCodeType" AS ENUM (
    'APPOINTMENT_SCHEDULED',
    'APPOINTMENT_RESCHEDULED',
    'APPOINTMENT_CANCELED',
    'APPOINTMENT_REMINDER',
    'APPOINTMENT_CONFIRMED'
);


ALTER TYPE public."AppointmentNotificationCodeType" OWNER TO appdbuser;

--
-- Name: AppointmentStatusType; Type: TYPE; Schema: public; Owner: appdbuser
--

CREATE TYPE public."AppointmentStatusType" AS ENUM (
    'SCHEDULED',
    'CONFIRMED',
    'CANCELED',
    'FINISHED',
    'RESCHEDULED'
);


ALTER TYPE public."AppointmentStatusType" OWNER TO appdbuser;

--
-- Name: AssistantStatusType; Type: TYPE; Schema: public; Owner: appdbuser
--

CREATE TYPE public."AssistantStatusType" AS ENUM (
    'ENABLED',
    'DISABLED',
    'DELETED'
);


ALTER TYPE public."AssistantStatusType" OWNER TO appdbuser;

--
-- Name: AvailabilityTimeSlotStatusType; Type: TYPE; Schema: public; Owner: appdbuser
--

CREATE TYPE public."AvailabilityTimeSlotStatusType" AS ENUM (
    'ENABLED',
    'DISABLED',
    'DELETED'
);


ALTER TYPE public."AvailabilityTimeSlotStatusType" OWNER TO appdbuser;

--
-- Name: ClientStatusType; Type: TYPE; Schema: public; Owner: appdbuser
--

CREATE TYPE public."ClientStatusType" AS ENUM (
    'ENABLED',
    'DISABLED',
    'DELETED'
);


ALTER TYPE public."ClientStatusType" OWNER TO appdbuser;

--
-- Name: GeneralNotificationCodeType; Type: TYPE; Schema: public; Owner: appdbuser
--

CREATE TYPE public."GeneralNotificationCodeType" AS ENUM (
    'GENERAL_WELCOME',
    'GENERAL_NEWS',
    'GENERAL_PROMOTION',
    'GENERAL_SERVICE_UPDATE'
);


ALTER TYPE public."GeneralNotificationCodeType" OWNER TO appdbuser;

--
-- Name: NotificationStatusType; Type: TYPE; Schema: public; Owner: appdbuser
--

CREATE TYPE public."NotificationStatusType" AS ENUM (
    'READ',
    'UNREAD'
);


ALTER TYPE public."NotificationStatusType" OWNER TO appdbuser;

--
-- Name: NotificationType; Type: TYPE; Schema: public; Owner: appdbuser
--

CREATE TYPE public."NotificationType" AS ENUM (
    'APPOINTMENT_NOTIFICATION',
    'PAYMENT_NOTIFICATION',
    'SYSTEM_NOTIFICATION',
    'GENERAL_NOTIFICATION'
);


ALTER TYPE public."NotificationType" OWNER TO appdbuser;

--
-- Name: RoleType; Type: TYPE; Schema: public; Owner: appdbuser
--

CREATE TYPE public."RoleType" AS ENUM (
    'ADMINISTRATOR',
    'CLIENT',
    'ASSISTANT'
);


ALTER TYPE public."RoleType" OWNER TO appdbuser;

--
-- Name: ServiceOfferStatusType; Type: TYPE; Schema: public; Owner: appdbuser
--

CREATE TYPE public."ServiceOfferStatusType" AS ENUM (
    'ENABLED',
    'DISABLED',
    'DELETED'
);


ALTER TYPE public."ServiceOfferStatusType" OWNER TO appdbuser;

--
-- Name: ServiceStatusType; Type: TYPE; Schema: public; Owner: appdbuser
--

CREATE TYPE public."ServiceStatusType" AS ENUM (
    'ENABLED',
    'DISABLED',
    'DELETED'
);


ALTER TYPE public."ServiceStatusType" OWNER TO appdbuser;

--
-- Name: SystemNotificationCodeType; Type: TYPE; Schema: public; Owner: appdbuser
--

CREATE TYPE public."SystemNotificationCodeType" AS ENUM (
    'SYSTEM_MAINTENANCE',
    'SYSTEM_ERROR',
    'SYSTEM_UPDATE',
    'SYSTEM_BACKUP',
    'SYSTEM_SECURITY',
    'SYSTEM_THRESHOLD',
    'SYSTEM_CONFIG_CHANGE'
);


ALTER TYPE public."SystemNotificationCodeType" OWNER TO appdbuser;

--
-- Name: SystemNotificationSeverityCodeType; Type: TYPE; Schema: public; Owner: appdbuser
--

CREATE TYPE public."SystemNotificationSeverityCodeType" AS ENUM (
    'INFO',
    'WARNING',
    'CRITICAL'
);


ALTER TYPE public."SystemNotificationSeverityCodeType" OWNER TO appdbuser;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Appointment; Type: TABLE; Schema: public; Owner: appdbuser
--

CREATE TABLE public."Appointment" (
    status public."AppointmentStatusType",
    total_cost numeric,
    uuid uuid,
    id integer DEFAULT nextval(('"appointment_id_seq"'::text)::regclass) NOT NULL,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    id_client integer,
    start_date timestamp with time zone,
    end_date timestamp with time zone
);


ALTER TABLE public."Appointment" OWNER TO appdbuser;

--
-- Name: AppointmentNotification; Type: TABLE; Schema: public; Owner: appdbuser
--

CREATE TABLE public."AppointmentNotification" (
    id_appointment integer,
    id_notification integer,
    code public."AppointmentNotificationCodeType"
);


ALTER TABLE public."AppointmentNotification" OWNER TO appdbuser;

--
-- Name: Assistant; Type: TABLE; Schema: public; Owner: appdbuser
--

CREATE TABLE public."Assistant" (
    id_user_account integer NOT NULL,
    status public."AssistantStatusType"
);


ALTER TABLE public."Assistant" OWNER TO appdbuser;

--
-- Name: AvailabilityTimeSlot; Type: TABLE; Schema: public; Owner: appdbuser
--

CREATE TABLE public."AvailabilityTimeSlot" (
    id integer DEFAULT nextval(('"availabilitytimeslot_id_seq"'::text)::regclass) NOT NULL,
    uuid uuid,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    id_assistant integer NOT NULL,
    status public."AvailabilityTimeSlotStatusType",
    start_date timestamp with time zone,
    end_date timestamp with time zone
);


ALTER TABLE public."AvailabilityTimeSlot" OWNER TO appdbuser;

--
-- Name: Client; Type: TABLE; Schema: public; Owner: appdbuser
--

CREATE TABLE public."Client" (
    id_user_account integer NOT NULL,
    status public."ClientStatusType"
);


ALTER TABLE public."Client" OWNER TO appdbuser;

--
-- Name: GeneralNotification; Type: TABLE; Schema: public; Owner: appdbuser
--

CREATE TABLE public."GeneralNotification" (
    code public."GeneralNotificationCodeType",
    id_notification integer
);


ALTER TABLE public."GeneralNotification" OWNER TO appdbuser;

--
-- Name: NotificationBase; Type: TABLE; Schema: public; Owner: appdbuser
--

CREATE TABLE public."NotificationBase" (
    id integer DEFAULT nextval(('"notificationbase_id_seq"'::text)::regclass) NOT NULL,
    uuid uuid,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    message text,
    type public."NotificationType"
);


ALTER TABLE public."NotificationBase" OWNER TO appdbuser;

--
-- Name: NotificationRecipient; Type: TABLE; Schema: public; Owner: appdbuser
--

CREATE TABLE public."NotificationRecipient" (
    id_user_account integer,
    id_notification integer,
    status public."NotificationStatusType",
    changed_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public."NotificationRecipient" OWNER TO appdbuser;

--
-- Name: ScheduledService; Type: TABLE; Schema: public; Owner: appdbuser
--

CREATE TABLE public."ScheduledService" (
    "id_serviceOffer" integer,
    id_appointment integer,
    start_date timestamp with time zone,
    end_date timestamp with time zone,
    service_name character varying(100),
    service_price double precision,
    service_minutes integer,
    uuid uuid,
    id integer DEFAULT nextval(('"scheduledservice_id_seq"'::text)::regclass) NOT NULL
);


ALTER TABLE public."ScheduledService" OWNER TO appdbuser;

--
-- Name: Service; Type: TABLE; Schema: public; Owner: appdbuser
--

CREATE TABLE public."Service" (
    description text,
    minutes integer,
    name character varying(100),
    price double precision,
    id integer DEFAULT nextval(('"service_id_seq"'::text)::regclass) NOT NULL,
    uuid uuid,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    status public."ServiceStatusType"
);


ALTER TABLE public."Service" OWNER TO appdbuser;

--
-- Name: ServiceOffer; Type: TABLE; Schema: public; Owner: appdbuser
--

CREATE TABLE public."ServiceOffer" (
    id_assistant integer,
    id_service integer,
    uuid uuid,
    id integer DEFAULT nextval(('"serviceoffer_id_seq"'::text)::regclass) NOT NULL,
    status public."ServiceOfferStatusType"
);


ALTER TABLE public."ServiceOffer" OWNER TO appdbuser;

--
-- Name: SystemNotification; Type: TABLE; Schema: public; Owner: appdbuser
--

CREATE TABLE public."SystemNotification" (
    severity public."SystemNotificationSeverityCodeType",
    code public."SystemNotificationCodeType",
    id_notification integer
);


ALTER TABLE public."SystemNotification" OWNER TO appdbuser;

--
-- Name: UnavailableTimeSlot; Type: TABLE; Schema: public; Owner: appdbuser
--

CREATE TABLE public."UnavailableTimeSlot" (
    start_date timestamp with time zone,
    end_date timestamp with time zone,
    id_availability_time_slot integer
);


ALTER TABLE public."UnavailableTimeSlot" OWNER TO appdbuser;

--
-- Name: UserAccount; Type: TABLE; Schema: public; Owner: appdbuser
--

CREATE TABLE public."UserAccount" (
    email character varying(100),
    password character varying(100),
    username character varying(50),
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    id integer DEFAULT nextval(('"useraccount_id_seq"'::text)::regclass) NOT NULL,
    role public."RoleType",
    uuid uuid,
    status public."AccountStatusType"
);


ALTER TABLE public."UserAccount" OWNER TO appdbuser;

--
-- Name: UserInformation; Type: TABLE; Schema: public; Owner: appdbuser
--

CREATE TABLE public."UserInformation" (
    name character varying(100),
    phone_number character varying(15),
    filepath character varying(255),
    id_user integer NOT NULL
);


ALTER TABLE public."UserInformation" OWNER TO appdbuser;

--
-- Name: appointment_id_seq; Type: SEQUENCE; Schema: public; Owner: appdbuser
--

CREATE SEQUENCE public.appointment_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.appointment_id_seq OWNER TO appdbuser;

--
-- Name: availabilitytimeslot_id_seq; Type: SEQUENCE; Schema: public; Owner: appdbuser
--

CREATE SEQUENCE public.availabilitytimeslot_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.availabilitytimeslot_id_seq OWNER TO appdbuser;

--
-- Name: notificationbase_id_seq; Type: SEQUENCE; Schema: public; Owner: appdbuser
--

CREATE SEQUENCE public.notificationbase_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.notificationbase_id_seq OWNER TO appdbuser;

--
-- Name: scheduledservice_id_seq; Type: SEQUENCE; Schema: public; Owner: appdbuser
--

CREATE SEQUENCE public.scheduledservice_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.scheduledservice_id_seq OWNER TO appdbuser;

--
-- Name: service_id_seq; Type: SEQUENCE; Schema: public; Owner: appdbuser
--

CREATE SEQUENCE public.service_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.service_id_seq OWNER TO appdbuser;

--
-- Name: serviceoffer_id_seq; Type: SEQUENCE; Schema: public; Owner: appdbuser
--

CREATE SEQUENCE public.serviceoffer_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.serviceoffer_id_seq OWNER TO appdbuser;

--
-- Name: useraccount_id_seq; Type: SEQUENCE; Schema: public; Owner: appdbuser
--

CREATE SEQUENCE public.useraccount_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.useraccount_id_seq OWNER TO appdbuser;

--
-- Data for Name: Appointment; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."Appointment" (status, total_cost, uuid, id, created_at, id_client, start_date, end_date) FROM stdin;
CANCELED	1500	01996ecb-3160-7307-b1b4-203540b53afc	68	2025-09-22 00:20:34.331467+00	221	2025-09-21 17:00:00+00	2025-09-21 18:00:00+00
CANCELED	1500	01996e50-e716-797e-96f0-5cacf3f156f0	67	2025-09-21 22:06:59.890002+00	221	2025-09-21 16:00:00+00	2025-09-21 17:00:00+00
CANCELED	1500	01993b7b-6da7-7527-a902-467180e03a68	55	2025-09-12 01:12:49.043261+00	221	2025-09-12 22:00:00+00	2025-09-12 23:00:00+00
CANCELED	2300	01993c72-7495-767b-bab0-c59cd6b84498	56	2025-09-12 05:42:37.980472+00	221	2025-09-12 19:00:00+00	2025-09-12 20:40:00+00
FINISHED	900	01993b7a-4a2a-74c8-a423-1aa743556e6d	54	2025-09-12 01:11:34.374061+00	221	2025-09-12 20:00:00+00	2025-09-12 20:45:00+00
CONFIRMED	1500	0199363e-3a16-77a6-9576-650e806e0c40	50	2025-09-11 00:47:51.829064+00	215	2025-09-12 15:00:00+00	2025-09-12 16:00:00+00
CONFIRMED	4500	01993a7c-a2c0-7292-8b7c-dee1002652fc	51	2025-09-11 20:34:30.749248+00	215	2025-09-12 16:50:00+00	2025-09-12 19:50:00+00
CONFIRMED	4500	01993b0c-675a-7bcc-8249-f62ecd1f3b67	52	2025-09-11 23:11:32.701033+00	217	2025-09-12 17:50:00+00	2025-09-12 20:50:00+00
CONFIRMED	2400	01993b4f-878d-7b4a-a991-057438192afa	53	2025-09-12 00:24:51.853386+00	217	2025-09-12 15:00:00+00	2025-09-12 16:45:00+00
CONFIRMED	1500	019950a0-e87b-7d6e-bece-4b68d26a66a0	57	2025-09-16 03:45:46.648101+00	217	2025-09-17 19:00:00+00	2025-09-17 20:00:00+00
CONFIRMED	1500	0199513d-4dc4-793f-ab0d-0e13e0c914ba	58	2025-09-16 06:36:36.337047+00	215	2025-09-17 15:00:00+00	2025-09-17 16:00:00+00
CONFIRMED	900	019954d8-dc58-76e1-b871-dec8a0287900	59	2025-09-16 23:25:22.427761+00	216	2025-09-16 15:00:00+00	2025-09-16 15:45:00+00
CONFIRMED	1500	019954e1-ec7e-7cd4-9b89-6d255e8c5b90	60	2025-09-16 23:35:16.349469+00	217	2025-09-16 22:00:00+00	2025-09-16 23:00:00+00
CANCELED	1500	019968ab-89b8-75aa-a5cf-5611023db300	61	2025-09-20 19:48:16.461941+00	216	2025-09-20 15:00:00+00	2025-09-20 16:00:00+00
FINISHED	900	019968fa-6ce7-7e4b-ac9d-047028993237	63	2025-09-20 21:14:26.550124+00	217	2025-09-20 15:00:00+00	2025-09-20 15:45:00+00
FINISHED	900	0199691f-43fa-7bb1-957b-bf028ab50d16	64	2025-09-20 21:54:40.886996+00	217	2025-09-20 15:45:00+00	2025-09-20 16:30:00+00
CANCELED	900	01996929-55d6-7919-8176-e9080375c5cb	65	2025-09-20 22:05:40.813336+00	215	2025-09-20 16:30:00+00	2025-09-20 17:15:00+00
CANCELED	1500	019968ac-854f-7b90-b201-182f1934d37c	62	2025-09-20 19:49:20.849514+00	221	2025-09-21 15:00:00+00	2025-09-21 16:00:00+00
CONFIRMED	1500	01996e2e-9e6b-7382-bbca-4ca477690753	66	2025-09-21 21:29:33.155202+00	216	2025-09-21 15:00:00+00	2025-09-21 16:00:00+00
FINISHED	1200	01996f14-9b60-79d5-8181-38372d5e0c57	70	2025-09-22 01:40:45.537798+00	221	2025-09-23 19:00:00+00	2025-09-23 20:00:00+00
CANCELED	3500	01996f1e-5c31-70ed-83c8-6cdd0aa7278a	71	2025-09-22 01:51:24.722323+00	221	2025-09-23 20:00:00+00	2025-09-23 22:40:00+00
CANCELED	1200	01996f11-bc16-7ae9-9a4d-56bccd285579	69	2025-09-22 01:37:37.318474+00	221	2025-09-23 15:00:00+00	2025-09-23 16:00:00+00
FINISHED	3500	01996f22-f5e3-7d0e-a15f-91bc96352fc9	72	2025-09-22 01:56:26.212871+00	221	2025-09-23 20:00:00+00	2025-09-23 22:40:00+00
CONFIRMED	1200	01996f2c-309b-76e1-b17d-3d8c0bdcdac8	73	2025-09-22 02:06:31.069275+00	221	2025-09-24 18:00:00+00	2025-09-24 19:00:00+00
\.


--
-- Data for Name: AppointmentNotification; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."AppointmentNotification" (id_appointment, id_notification, code) FROM stdin;
\.


--
-- Data for Name: Assistant; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."Assistant" (id_user_account, status) FROM stdin;
218	\N
219	\N
220	\N
\.


--
-- Data for Name: AvailabilityTimeSlot; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."AvailabilityTimeSlot" (id, uuid, created_at, id_assistant, status, start_date, end_date) FROM stdin;
63	01993604-0a14-783b-998b-6ed971602f45	2025-09-10 23:44:18.455292+00	219	ENABLED	2025-09-11 15:00:00+00	2025-09-12 03:00:00+00
64	01993604-1da8-7448-81cc-6a744fc8b17d	2025-09-10 23:44:23.466828+00	220	ENABLED	2025-09-11 15:00:00+00	2025-09-12 03:00:00+00
66	01993a79-4214-7cb7-ba3a-a44ee7221b4d	2025-09-11 20:30:49.55202+00	219	ENABLED	2025-09-12 15:00:00+00	2025-09-12 23:00:00+00
67	01993a79-7a5d-726c-952c-bf51d4831a7e	2025-09-11 20:31:03.782141+00	220	ENABLED	2025-09-12 15:00:00+00	2025-09-12 23:00:00+00
69	0199503d-8938-7db7-b47d-b31e652e4276	2025-09-16 01:57:14.172722+00	219	ENABLED	2025-09-16 15:00:00+00	2025-09-16 23:00:00+00
70	0199503d-9527-7b54-9029-074f5e2d7c7e	2025-09-16 01:57:17.265989+00	220	ENABLED	2025-09-16 15:00:00+00	2025-09-16 23:00:00+00
71	0199503f-bddc-75be-b08b-d9b4c9594f4a	2025-09-16 01:59:38.718482+00	220	ENABLED	2025-09-17 15:00:00+00	2025-09-17 23:00:00+00
65	01993628-5cad-7b6c-93f2-c305e6f3d2e3	2025-09-11 00:23:58.894371+00	218	ENABLED	2025-09-12 15:00:00+00	2025-09-13 03:00:00+00
68	0199503d-7612-7be4-946b-6e1d0f3302c0	2025-09-16 01:57:09.531108+00	218	ENABLED	2025-09-16 15:00:00+00	2025-09-16 23:00:00+00
72	019954e4-31ed-712e-b062-f1af3c5f960b	2025-09-16 23:37:45.230275+00	219	ENABLED	2025-09-18 15:00:00+00	2025-09-19 03:00:00+00
73	019968a9-8b22-78ae-8e31-32934c26c608	2025-09-20 19:46:05.854032+00	218	ENABLED	2025-09-20 15:00:00+00	2025-09-20 23:00:00+00
74	019968ac-5610-7fa6-86bb-eeef313ec268	2025-09-20 19:49:08.772681+00	219	ENABLED	2025-09-21 15:00:00+00	2025-09-21 23:00:00+00
75	01996f10-9608-77c2-bf9d-970d1fc391ae	2025-09-22 01:36:22.21691+00	219	ENABLED	2025-09-23 15:00:00+00	2025-09-23 23:00:00+00
76	01996f2b-ac7c-710d-b411-ebab8dd5ecd7	2025-09-22 02:05:57.246305+00	220	ENABLED	2025-09-24 15:00:00+00	2025-09-24 23:00:00+00
62	01993603-e4ea-76b3-ba72-46b2a9f5a22d	2025-09-10 23:44:08.963603+00	218	ENABLED	2025-09-11 15:00:00+00	2025-09-12 03:00:00+00
\.


--
-- Data for Name: Client; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."Client" (id_user_account, status) FROM stdin;
215	\N
216	\N
217	\N
221	\N
\.


--
-- Data for Name: GeneralNotification; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."GeneralNotification" (code, id_notification) FROM stdin;
\.


--
-- Data for Name: NotificationBase; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."NotificationBase" (id, uuid, created_at, message, type) FROM stdin;
\.


--
-- Data for Name: NotificationRecipient; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."NotificationRecipient" (id_user_account, id_notification, status, changed_at) FROM stdin;
\.


--
-- Data for Name: ScheduledService; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."ScheduledService" ("id_serviceOffer", id_appointment, start_date, end_date, service_name, service_price, service_minutes, uuid, id) FROM stdin;
250	50	2025-09-12 15:00:00+00	2025-09-12 16:00:00+00	Asesoria fiscal	1500	60	0199363e-3a16-7932-9000-26aab36edbc0	67
250	51	2025-09-12 16:50:00+00	2025-09-12 17:50:00+00	Asesoria fiscal	1500	60	01993a7c-a2c0-7acb-8406-533e4cb95890	68
251	51	2025-09-12 17:50:00+00	2025-09-12 18:50:00+00	Asesoria fiscal	1500	60	01993a7c-a2c0-75e0-8d00-254046874ac4	69
252	51	2025-09-12 18:50:00+00	2025-09-12 19:50:00+00	Asesoria fiscal	1500	60	01993a7c-a2c0-7630-a7b8-2f2522040745	70
250	52	2025-09-12 17:50:00+00	2025-09-12 18:50:00+00	Asesoria fiscal	1500	60	01993b0c-675a-7284-be6d-552d0b166bd0	71
251	52	2025-09-12 18:50:00+00	2025-09-12 19:50:00+00	Asesoria fiscal	1500	60	01993b0c-675a-75f2-bf55-a52494e57b0d	72
252	52	2025-09-12 19:50:00+00	2025-09-12 20:50:00+00	Asesoria fiscal	1500	60	01993b0c-675a-74da-b16b-b1c66a7930df	73
251	53	2025-09-12 15:00:00+00	2025-09-12 16:00:00+00	Asesoria fiscal	1500	60	01993b4f-878d-7ab1-96c6-1bf7c62eb183	74
259	53	2025-09-12 16:00:00+00	2025-09-12 16:45:00+00	Asesoramiento de inversiones	900	45	01993b4f-878d-7347-b259-3c44818ccfc2	75
259	54	2025-09-12 20:00:00+00	2025-09-12 20:45:00+00	Asesoramiento de inversiones	900	45	01993b7a-4a2a-7d9a-b6ca-4cbbccc9c034	76
250	55	2025-09-12 22:00:00+00	2025-09-12 23:00:00+00	Asesoria fiscal	1500	60	01993b7b-6da7-753a-9573-910f3ef302f2	77
250	56	2025-09-12 19:00:00+00	2025-09-12 20:00:00+00	Asesoria fiscal	1500	60	01993c72-7495-716a-9531-b482de2ef29d	78
257	56	2025-09-12 20:00:00+00	2025-09-12 20:40:00+00	Plan de ahorro	800	40	01993c72-7495-79e0-92c0-a700e7cbc86d	79
252	57	2025-09-17 19:00:00+00	2025-09-17 20:00:00+00	Asesoria fiscal	1500	60	019950a0-e87b-7353-806f-0a6da2268748	80
252	58	2025-09-17 15:00:00+00	2025-09-17 16:00:00+00	Asesoria fiscal	1500	60	0199513d-4dc4-727f-b925-dbf0ca07be3d	81
259	59	2025-09-16 15:00:00+00	2025-09-16 15:45:00+00	Asesoramiento de inversiones	900	45	019954d8-dc58-79d3-9dc7-33c8a04eff84	82
252	60	2025-09-16 22:00:00+00	2025-09-16 23:00:00+00	Asesoria fiscal	1500	60	019954e1-ec7e-7a29-af24-32509552eff6	83
250	61	2025-09-20 15:00:00+00	2025-09-20 16:00:00+00	Asesoria fiscal	1500	60	019968ab-89b8-7357-99ea-2f489bd2bc0d	84
251	62	2025-09-21 15:00:00+00	2025-09-21 16:00:00+00	Asesoria fiscal	1500	60	019968ac-854f-75c5-9afb-0277ef3946e2	85
259	63	2025-09-20 15:00:00+00	2025-09-20 15:45:00+00	Asesoramiento de inversiones	900	45	019968fa-6ce7-78ab-b9f2-4168ca8ce1d3	86
259	64	2025-09-20 15:45:00+00	2025-09-20 16:30:00+00	Asesoramiento de inversiones	900	45	0199691f-43fa-77f3-8894-1a04c1e39f01	87
259	65	2025-09-20 16:30:00+00	2025-09-20 17:15:00+00	Asesoramiento de inversiones	900	45	01996929-55d6-788c-9c73-e78b026a3ab5	88
251	66	2025-09-21 15:00:00+00	2025-09-21 16:00:00+00	Asesoria fiscal	1500	60	01996e2e-9e6b-77b4-8f2f-a6e02779db76	89
251	67	2025-09-21 16:00:00+00	2025-09-21 17:00:00+00	Asesoria fiscal	1500	60	01996e50-e716-7e94-81fc-cf0cf1df8e5c	90
251	68	2025-09-21 17:00:00+00	2025-09-21 18:00:00+00	Asesoria fiscal	1500	60	01996ecb-3160-7ca9-ad4c-b09e3bd5c768	91
258	69	2025-09-23 15:00:00+00	2025-09-23 16:00:00+00	Planificacion financiera	1200	60	01996f11-bc16-7810-a30b-69292deb2df4	92
258	70	2025-09-23 19:00:00+00	2025-09-23 20:00:00+00	Planificacion financiera	1200	60	01996f14-9b60-7d3b-9a2f-8a8d5a11ec4d	93
258	71	2025-09-23 20:00:00+00	2025-09-23 21:00:00+00	Planificacion financiera	1200	60	01996f1e-5c31-7deb-9827-0ea1a35ee17b	94
257	71	2025-09-23 21:00:00+00	2025-09-23 21:40:00+00	Plan de ahorro	800	40	01996f1e-5c31-76ed-b5fa-db710493c05e	95
251	71	2025-09-23 21:40:00+00	2025-09-23 22:40:00+00	Asesoria fiscal	1500	60	01996f1e-5c31-72ce-9b9e-41191c582091	96
251	72	2025-09-23 20:00:00+00	2025-09-23 21:00:00+00	Asesoria fiscal	1500	60	01996f22-f5e3-7fbe-b864-ee0897750ee8	97
257	72	2025-09-23 21:00:00+00	2025-09-23 21:40:00+00	Plan de ahorro	800	40	01996f22-f5e3-7568-800d-a0a33aa0b903	98
258	72	2025-09-23 21:40:00+00	2025-09-23 22:40:00+00	Planificacion financiera	1200	60	01996f22-f5e3-7bce-be3b-d375457afec3	99
270	73	2025-09-24 18:00:00+00	2025-09-24 19:00:00+00	Planificacion financiera	1200	60	01996f2c-309b-7378-866a-77a90beab642	100
\.


--
-- Data for Name: Service; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."Service" (description, minutes, name, price, id, uuid, created_at, status) FROM stdin;
Revision de obligaciones fiscales y recomendaciones para optimizar pagos y deducciones	60	Asesoria fiscal	1500	17	01993602-79a0-7e0d-af24-157d78c86a52	2025-09-10 23:42:35.937609+00	ENABLED
Elaboracion de un plan de ahorro adaptado a los ingresos y metas del cliente	40	Plan de ahorro	800	16	01993602-2376-71d3-ac19-786207bf7064	2025-09-10 23:42:13.880116+00	ENABLED
Asesoramiento sobre obligaciones fiscales internacionales y optimizacion de impuestos en distintos paises.	60	Consultoria tributaria internacional	2000	18	01996f29-9c4e-7881-8e33-15fe9888d74f	2025-09-22 02:03:42.050996+00	ENABLED
Diseno de estrategias para proteger y distribuir el patrimonio familiar o empresarial.	60	Planificacion patrimonial	1800	19	01996f2a-15ff-77d8-bd98-9a92b44fb8bf	2025-09-22 02:04:13.187041+00	ENABLED
Evaluacion de polizas existentes y recomendaciones de seguros adecuados para cubrir riesgos personales o empresariales.	120	Asesoria en seguros	700	20	01996f2a-cf24-7475-9a21-77896e452ef7	2025-09-22 02:05:00.581808+00	ENABLED
Analisis detallado de los gastos del cliente y estrategias para reducir costos innecesarios.	60	Optimizacion de gastos	750	21	01996f2b-0e2e-7969-a838-ef93bb760987	2025-09-22 02:05:16.719608+00	ENABLED
Orientacion sobre oportunidades de inversion en bienes raices segun perfil y objetivos del cliente.	1100	Asesoria en inversiones inmobiliarias	60	22	01996f2b-5635-7e04-a750-ee9d2d64837e	2025-09-22 02:05:35.156324+00	ENABLED
Evaluacion de la situacion financiera y creacion de un plan personalizado para alcanzar objetivos economicos	60	Planificacion financiera	1200	13	01993601-54e1-787c-b173-66d38cc71eb9	2025-09-10 23:41:21.012118+00	ENABLED
Orientacion sobre opciones de inversion seguras y rentables segun el perfil del cliente	45	Asesoramiento de inversiones	900	14	01993601-a167-7910-90cd-97b0bf05ac49	2025-09-10 23:41:40.585755+00	ENABLED
Analisis de deudas existentes y estrategias para reducirlas de manera efectiva	50	Gestion de deudas	1000	15	01993601-e1a2-7390-8743-50c7ca3615ca	2025-09-10 23:41:57.031188+00	ENABLED
\.


--
-- Data for Name: ServiceOffer; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."ServiceOffer" (id_assistant, id_service, uuid, id, status) FROM stdin;
218	17	01993602-af14-7cbf-a20f-8d26a79864bf	250	ENABLED
219	17	01993602-c34c-72c4-b314-d0121be6ab8c	251	ENABLED
218	13	01993b31-5a17-7749-92ba-7106de39b3de	254	DELETED
218	14	01993b31-5cb4-7db6-9ca7-a38dedc908cf	255	DELETED
218	15	01993b31-61ef-72d8-aedb-462b99ad09f8	256	DELETED
219	13	01993b35-5a5e-7baa-a397-4a9d6194c059	258	ENABLED
218	14	01993b38-877e-71ff-a6bb-caa978326e95	259	ENABLED
220	17	01993602-e7d4-74f4-85c7-2f837a46b47f	252	ENABLED
218	15	01993c7e-9f3e-7aa9-82d7-02b162bbed12	260	ENABLED
218	13	01993c7e-a2d4-7417-873f-9a9f07a45703	261	ENABLED
220	14	01995449-ee0c-7390-9c1f-0bffef8b77c8	262	ENABLED
220	15	01995449-f13e-7a91-bbed-71af91171129	263	ENABLED
218	16	01993b31-56a1-7917-a1b2-aee8a4e31f5c	253	ENABLED
219	16	01993b35-57db-73c9-b6f2-5fb9ec577f27	257	ENABLED
220	16	01996f2b-832d-7e6d-97f9-1fd7998889f1	264	ENABLED
220	18	01996f2b-8413-782d-a447-798a993df282	265	ENABLED
220	19	01996f2b-84e6-7a1d-8f6a-5121b47be1f9	266	ENABLED
220	20	01996f2b-858d-7d51-9033-08237dd9d2e6	267	ENABLED
220	21	01996f2b-862c-7429-8e29-ee8a237493a4	268	ENABLED
220	22	01996f2b-86c8-797a-9fb2-d212ce2e9ac0	269	ENABLED
220	13	01996f2b-877b-7efe-b5e6-bc2d7f13a1f9	270	ENABLED
\.


--
-- Data for Name: SystemNotification; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."SystemNotification" (severity, code, id_notification) FROM stdin;
\.


--
-- Data for Name: UnavailableTimeSlot; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."UnavailableTimeSlot" (start_date, end_date, id_availability_time_slot) FROM stdin;
2025-09-11 21:00:00+00	2025-09-11 22:00:00+00	62
2025-09-11 21:00:00+00	2025-09-11 22:00:00+00	63
2025-09-11 21:00:00+00	2025-09-11 22:00:00+00	64
2025-09-12 21:00:00+00	2025-09-12 22:00:00+00	65
2025-09-13 00:00:00+00	2025-09-13 01:00:00+00	65
2025-09-17 21:00:00+00	2025-09-17 22:00:00+00	71
\.


--
-- Data for Name: UserAccount; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."UserAccount" (email, password, username, created_at, id, role, uuid, status) FROM stdin;
adrian@hola.com	$2a$11$rbKmGlnbJI42FHOPLmz5Qe1RUFO6calh7PoWQzfOMzqkMIL.jbfly	adrian	2025-08-23 00:34:12.542223+00	1	ADMINISTRATOR	0198d458-e077-7856-a908-d73cb06bd670	ENABLED
Murl_Jones28@gmail.com	$2a$11$8iCz/YlkmxHa0qW0NTT8H.u1WXdyd0FhdI4QW/htiB.KDiWnoI9Aa	Bobby.Rodriguez	2025-09-09 22:01:08.425369+00	41	CLIENT	0199307f-395c-7011-ac3d-3d225f29feb7	ENABLED
Darion.Stoltenberg@hotmail.com	$2a$11$vn061cSftpwQJyHDqL6yhueR1/EZ2FuIaue63eJK/vvau4Fv8AFkq	Carmelo.Wyman	2025-09-09 22:01:08.763356+00	42	CLIENT	0199307f-3a8d-7149-8612-9a89e981b016	ENABLED
Trycia.Hodkiewicz93@hotmail.com	$2a$11$M9nBDQ6GP8xC6mXrDSjw7eTfoASYsm87LmesYfGaY0pQf2IeTPXY.	Mina.Hagenes	2025-09-09 22:01:09.133566+00	43	CLIENT	0199307f-3bfc-7512-990a-771d082f3873	ENABLED
Regan92a@yahoo.com	$2a$11$SwzevYh9AsvUZqzwgCeLcuAH.hQKj3s5d6m95Ip.u3v/L3U9QNWOG	Mavis0	2025-08-23 00:34:44.413449+00	3	CLIENT	0198d459-614b-7de2-89c0-cfeadef950d7	ENABLED
Joanie_Hackett76@yahoo.com	$2a$11$0U6kKzwk/JA6oBvtrLZcI.sJwaiyo9NPtBI2vvd9qxZPItxJUuI2q	Shannon.Russel77	2025-09-09 22:00:34.584099+00	13	CLIENT	0199307e-b4c0-713e-ab01-9a503d8333a1	ENABLED
Rebecca.Koch20@yahoo.com	$2a$11$L7Zg4C7pkcSbGlNc2YNQ0Oo2CQd42xwJfwksjuLPYhs7QcsTCf9WG	Eliane.Olson25	2025-09-09 22:00:36.09112+00	14	CLIENT	0199307e-bb23-7463-a082-a0f70257062d	ENABLED
Zachary_Upton@hotmail.com	$2a$11$FHSdZlGHF0oR0vajFSKtcO4x0VgM2mnLYL29wOY0g1RvShwJdI3I6	Iliana_Jerde0	2025-09-09 22:00:37.357181+00	15	CLIENT	0199307e-bff7-71fd-a2fc-19a465e2dc6e	ENABLED
Dwight0@hotmail.com	$2a$11$fjL4Tq2sjz8wowNsd3QTwOJZnrSVGHc02.CBxWuYpwCWcemwZaVrG	Adeline71	2025-09-09 22:01:00.197729+00	16	CLIENT	0199307f-18a0-77f2-b30a-0d389c9daca2	ENABLED
Queen.Adams@hotmail.com	$2a$11$TKrvlDcNLYixfUdPR.Tb1eXRxJyWR0Z8kuM3SEgx27oEugYL0A9VK	Martin20	2025-09-09 22:01:00.577629+00	17	CLIENT	0199307f-1a75-79ae-b8c4-e045efdda375	ENABLED
Angus63@gmail.com	$2a$11$ZxRsELhaUuooK/D3.DefQeReL4oBLhVPVinfoHsIitKB.t9Md3fnO	Deborah.Romaguera	2025-09-09 22:01:00.919518+00	18	CLIENT	0199307f-1c0e-7070-99e8-505aea0622f2	ENABLED
Yadira.Haley@yahoo.com	$2a$11$hQeHzWkN6QYioTEHW5zsyuDja8yuw4jzTPbGszsy9HrPRRCupTWae	Rhoda70	2025-09-09 22:01:01.283403+00	19	CLIENT	0199307f-1db7-7459-8d71-fd748075cfdb	ENABLED
Noemi2@yahoo.com	$2a$11$GSYqdBYAJCjKqdkF5PVvneGAd2LPzf6U37nFnfKtk1VqYfCpNbtni	Vena_Kilback59	2025-09-09 22:01:01.573582+00	20	CLIENT	0199307f-1ea8-703c-9cf3-ca63cbe4576e	ENABLED
Chaya_Ruecker23@yahoo.com	$2a$11$c9Zjws.Iso/jcMyt4gg3x.FTZfcYHQSd/cmNrjb7Uqm4qvpxm4y6y	Caitlyn_Wyman8	2025-09-09 22:01:01.883354+00	21	CLIENT	0199307f-1fe4-79ff-9644-7af8c0d43f4b	ENABLED
Lucio_Dickinson30@yahoo.com	$2a$11$1l1VXYuiM5icxTYTXptpr.7GUS9h7RrKQ1uZeHmUTUpzvC6tyc2E6	Guadalupe19	2025-09-09 22:01:02.183614+00	22	CLIENT	0199307f-2105-755f-ad32-d5d793bbd5ab	ENABLED
Santa_Ward@gmail.com	$2a$11$QXJq4YPyPMeoNqn9.HTya.wf6GPO6Nqk8/9NOdzzw6l3.a3WUXeta	Eli.Baumbach	2025-09-09 22:01:02.466921+00	23	CLIENT	0199307f-223b-70fa-9983-20d57d0e51a9	ENABLED
Junius24@hotmail.com	$2a$11$VPDQbBgNUGeBlTHyGNL28.1T7ImOt/jumupNopYGAall.dB5MSzAa	Jamie_Erdman	2025-09-09 22:01:02.754487+00	24	CLIENT	0199307f-233d-7cf6-ac79-21d0e1abfc54	ENABLED
Clemmie_Rippin45@gmail.com	$2a$11$1NmlcODrj4eH77Kv5xJhP.HReUgVp94qm2nKS5QK3vLHovQ3ZMJtm	Deondre.Spinka	2025-09-09 22:01:03.138501+00	25	CLIENT	0199307f-248a-70b5-a511-f1093b1fc0de	ENABLED
London.Gutkowski56@gmail.com	$2a$11$7vGd.atGRtFSEblkwyEed.mWNqMlRCu0TFHW9XrbmmPWhKpPugQkO	Boris_Mohr51	2025-09-09 22:01:03.421336+00	26	CLIENT	0199307f-25e1-72e6-a503-11b98b081c4d	ENABLED
Darron73@yahoo.com	$2a$11$WdUa91drDW70NYofgBwFLuA1g3zJOkqRvxMEBm0JwmCdzwl4sF6EC	Santino_OKon34	2025-09-09 22:01:03.70347+00	27	CLIENT	0199307f-2710-7eb3-91eb-8a072dae9e4f	ENABLED
Rex48@yahoo.com	$2a$11$KE4XpMq3NPKdr0i44cuB5uotUlwGfz3HQwv22ITq2aZH0xSzZUASS	Lexi.Beier	2025-09-09 22:01:03.967652+00	28	CLIENT	0199307f-280c-7810-978e-769d8f5d6331	ENABLED
Travon91@yahoo.com	$2a$11$nHljmZHfyBlz5dzo9jqEv.ouuTnwEh0uSSY7bnTuMh.DZFizwT38G	Mauricio_Crist86	2025-09-09 22:01:04.324838+00	29	CLIENT	0199307f-2945-7553-b7b6-87c9f2e9ee82	ENABLED
Lora.Hauck@hotmail.com	$2a$11$iZral6MDHcLajazM35JsA.GSaWVkB5LC3t8SovSZUl67h2/s3oRQC	Frederic_Brekke28	2025-09-09 22:01:04.649012+00	30	CLIENT	0199307f-2a82-7226-b8c5-6f485eb01385	ENABLED
Desmond_Hirthe@hotmail.com	$2a$11$F2O.3tijv22hYqcuooG06uUp6KcoJ69BrQ2juSx067bcZWSwXQluG	Jermaine36	2025-09-09 22:01:04.946597+00	31	CLIENT	0199307f-2bde-7cc9-94df-b1629261f5aa	ENABLED
Gaylord_Dietrich77@yahoo.com	$2a$11$Jzvxourhop/WhI4jz6EIo.dABNYgf9Bse62Y6yRgxh9dQFDiMjAEe	Golda.Morissette	2025-09-09 22:01:05.283219+00	32	CLIENT	0199307f-2cf3-7338-8325-e9935a0c9148	ENABLED
Osborne.Donnelly@gmail.com	$2a$11$z5IY8A9pfZzzMuAbyzTDi./eQDonIvuATKfhZ3gKHImiXLSE1XgWi	Lance39	2025-08-25 06:06:16.506729+00	7	ASSISTANT	0198dfd5-a0fb-7833-9908-a1db982653d7	ENABLED
Trevion.Torphy25@hotmail.com	$2a$11$5WBm44fbb8DUewgS9yq7Ruqs440ACMjC/JyTAEPi8ti0cm6rxQeie	Lottie14	2025-08-30 00:45:15.665424+00	9	CLIENT	0198f86f-874b-774b-b42b-12c0ddcc6574	ENABLED
Rachelle_Bergstrom67@gmail.com	$2a$11$JSDunk3dSeg8PDKM1Eee1e1x.YBoo8EARcdAfqGnIhoN9iDTbM5n.	Abby_Okuneva32	2025-08-30 00:45:17.021486+00	10	CLIENT	0198f86f-8caf-7ac6-b830-a85d1e4e00ac	ENABLED
Jaylan_Lueilwitz@gmail.com	$2a$11$eopl5/0E/U80/iu.X8KL5ebEdtmT.Lwq1OR1r8CWuBBXtcVbcwF12	Ricky.Doyle74	2025-09-09 22:01:05.578925+00	33	CLIENT	0199307f-2e5d-7296-a78c-e0293757d650	ENABLED
Alessandra.Von@yahoo.com	$2a$11$HhhTIqPdbVD9bs5gUzncHu7SMn1k5.jzQZ5BbkmdeLYu9jsyByIp2	Durward47	2025-08-30 00:45:18.270758+00	11	CLIENT	0198f86f-9194-7b29-859b-87e11cfeb9b6	DISABLED
Chris7@hotmail.com	$2a$11$pezT1NuDhY/l577yGLOFW.dzR4XzQo0gOEe34eHoIlodbaz2/e5yq	Quentin_Franecki18	2025-09-09 22:01:05.915767+00	34	CLIENT	0199307f-2f7c-7a14-8665-4041df1106ea	ENABLED
tester@teser.com	$2a$11$m/i0762uzwrqERvCtBMWHu3Ca1rYYKcbDKIvA0ERgRVMYwPPvjsuW	test	2025-09-03 00:32:08.467838+00	12	CLIENT	01990cfc-f287-7882-b384-3dbc8169b328	ENABLED
Elyse_Reynolds66@gmail.com	$2a$11$ifOuNft6AGCKOOVOzA9b8Oh99twFCf/8Abqai8NxDNfd8e4bCf7Ku	Micaela6	2025-09-09 22:01:06.289645+00	35	CLIENT	0199307f-30ea-7eff-b324-5108f792f611	ENABLED
Dortha.Schumm39@hotmail.com	$2a$11$A/9yTXIE2vZOayEISqLcAerFRDNqlJU1x1rPObxWhtHUMDQpeDxtO	Asa.Keeling84	2025-08-23 00:36:21.439452+00	4	ASSISTANT	0198d45a-dc82-7894-868f-9cb9a89384a9	ENABLED
Rudy2@yahoo.com	$2a$11$3JzGGKhA2YP2Jii0oc4hjuQQSeOTvVt6F3KLxY9CfBzpczoem1Xn2	Mattie_Padberg93	2025-09-09 22:01:06.624471+00	36	CLIENT	0199307f-322d-7889-a40b-f0dbab0eee0c	ENABLED
Alfredo_Parisian59@gmail.com	$2a$11$17MR0Kw.FahmjhdoHLlX0uu9N.g9mG6KRIwkTN5EkTHkSwplwMTL.	Belle_Bergstrom21	2025-08-25 23:30:19.815477+00	8	ASSISTANT	0198e391-7be1-7056-a83c-6e6e0ace8e45	ENABLED
Devon56@hotmail.com	$2a$11$ugd9ArDVUIWBpfNsqtJV9OcYbMLjf3pkk6GAmlz4vjHTlM4PhWqBG	Ubaldo_Hegmann	2025-08-25 05:46:35.323657+00	5	ASSISTANT	0198dfc3-9a1a-709a-b76a-c863152be0c3	ENABLED
Estevan_Dicki25@gmail.com	$2a$11$KzdYXRMT26xe3ByqB9gKLeZ2uSMQMIRaUMViMEgsZ13JPOU65Tm/S	Lucas_Will	2025-08-25 06:03:53.360208+00	6	ASSISTANT	0198dfd3-71de-7130-817a-8a4c851aea9e	ENABLED
Logan_Ryan62@gmail.com	$2a$11$/H/ME0zU50kEtoTvVrxHlOqWfm8jYT0sPZNGRCuE.JK8giNvvlGqq	Lillian.Boyer49	2025-09-09 22:01:09.495187+00	44	CLIENT	0199307f-3d75-73fb-b30c-4c4ae1b69913	ENABLED
Urban.Christiansen@gmail.com	$2a$11$vIhd3qHHySseNrQvRha7UeiENpqMo8TPDLjuVGlSO0D.t9K8No5k.	Yvonne_Crooks	2025-09-09 22:01:06.967662+00	37	CLIENT	0199307f-338f-7e32-8917-0fb950468716	ENABLED
Jude.Kovacek@hotmail.com	$2a$11$C8qPA.ohvraftWeweTsbvuhsIKbvziLuf/T3wogfyQYZl407/ZTTS	Mazie.Thompson	2025-09-09 22:01:07.290424+00	38	CLIENT	0199307f-34d6-7196-8630-5cd3c55119c9	ENABLED
Kiera.Runolfsdottir54@yahoo.com	$2a$11$VNdTYdMDoqvW14sHWboaO.LKnaVsc9tMhuVfYMFRPx3xvmTz./kxy	Abdullah75	2025-09-09 22:01:07.751245+00	39	CLIENT	0199307f-3693-7170-a242-8d3ea7338b5a	ENABLED
Lindsay.Murazik49@yahoo.com	$2a$11$sMNSldWU6f0R2BNMak5C7.PiRjrZTHpG9uhV7Wj.gzoojd9q2bZ2W	Garth.Tremblay	2025-09-09 22:01:08.114962+00	40	CLIENT	0199307f-3813-7a40-a4f4-22a9fca2d7d3	ENABLED
Emie41@hotmail.com	$2a$11$HTEu.NTjRXLWUdSljIJMHeRRX2ZezDC8c92b06WlpcyM33ViV9pPi	Jerome_Prosacco9	2025-09-09 22:01:09.814366+00	45	CLIENT	0199307f-3eb8-7fa7-a125-622b60b71193	ENABLED
Jaqueline4@gmail.com	$2a$11$OV1XoNoDDGkNDi/vqbCy3.enWpx0tZiCQ5IbBrZvTThaEO9.aiwmi	Frida.Monahan	2025-09-09 22:01:10.227094+00	46	CLIENT	0199307f-4032-7685-844d-a12ecef66ed8	ENABLED
Darrin77@hotmail.com	$2a$11$nzpKmfMCrEY0hEGwkG9UdOwtgzBzkcPLbCB5H9RZSrnPQgKSwgILK	Gunner86	2025-09-09 22:01:10.560481+00	47	CLIENT	0199307f-41b3-79ac-bbdf-422a4fc690da	ENABLED
Derek.Nolan@gmail.com	$2a$11$ey6BmzNc.V.k6mta0josgeSdZTsLiZivGBxnhV4GrfKlYBkjR82QK	Sunny2	2025-09-09 22:01:10.883007+00	48	CLIENT	0199307f-42f5-76e2-8f6f-7859510031d6	ENABLED
Domenico_Mohr@yahoo.com	$2a$11$7.KpiecIo8D.ZFkqqlfk0eGwXfvyctIHiisJL/uwRQJs1LPV4ThoO	Bernadine.Dicki	2025-09-09 22:01:11.259197+00	49	CLIENT	0199307f-4438-7149-ab88-847b09cd5381	ENABLED
Stevie41@hotmail.com	$2a$11$HFxPcG/6o9/L8ouxr70q2.YOF/fvK2KKgBrYzG/fwS6Pd/NwPKR0.	Rusty97	2025-09-09 22:01:11.569802+00	50	CLIENT	0199307f-45a3-7421-8ca8-5c07120451b1	ENABLED
Sabryna_Hauck@hotmail.com	$2a$11$kgJQx/ye625LKkzkWSpTnug54zJ0FCzKMN1khoDePvaUlyGP34DBa	Verla.OKeefe69	2025-09-09 22:01:11.915908+00	51	CLIENT	0199307f-46f3-79c7-8e94-f39290414929	ENABLED
Alexandre7@yahoo.com	$2a$11$uC0.8tO77UMwW3Rudrgo3umcb1LYEkxv.G1nUdWlQfYgFvGy/NcW.	Lavada.Funk	2025-09-09 22:01:12.239736+00	52	CLIENT	0199307f-481f-7882-9f63-63457d581cc8	ENABLED
Joelle.Lowe@gmail.com	$2a$11$rq6NsldRPDLh5CB6cF.hC.yfA8TQiLV6754jm0tqrDcTndsjJuGfS	Gideon.Buckridge	2025-09-09 22:01:12.578335+00	53	CLIENT	0199307f-4996-7bc5-9647-edbf7e735f76	ENABLED
Tyshawn.Hand48@gmail.com	$2a$11$etMN4fioj9gLM1CQflfaeO8L/44cH9gxD50FZf7hovxlPidBau2oG	Idella47	2025-09-09 22:01:12.87452+00	54	CLIENT	0199307f-4ac3-796d-bf22-63178727b131	ENABLED
Peter_Herman97@hotmail.com	$2a$11$KOd1g7Fn5tk8C1qDG3HHUe4ZtLcF5gAR6sD3hYry1mvBPI.eh7GFe	Jennings_Kilback63	2025-09-09 22:01:13.218559+00	55	CLIENT	0199307f-4c08-7f32-8a8b-5ca1e48df2e2	ENABLED
Veda27@yahoo.com	$2a$11$PzSPIzLhh/oVe752jMhOV.DU4r9u/Oo8o1YXYrd8gAtyMkgDV/mp2	Alivia32	2025-09-09 22:01:13.555699+00	56	CLIENT	0199307f-4d4c-71b3-8f1f-e64c01006ade	ENABLED
Luther_Ratke@yahoo.com	$2a$11$OX9U6T63MKewTxRFFekv8Oh6IoUosW0yVnsqat1TjSIoJVJ0aj0lq	Makenzie_Runolfsdottir20	2025-09-09 22:01:13.865953+00	57	CLIENT	0199307f-4e8c-707f-9109-4c5dacb74134	ENABLED
Diamond91@gmail.com	$2a$11$JbjDknXExSIAlxhEmHOlU.Ad8OItEaRbS0eeoJG0J9.na1yn8cUle	Eulah80	2025-09-09 22:01:14.237288+00	58	CLIENT	0199307f-4fec-763d-8430-2f7bd7e25845	ENABLED
Darrick_Batz@yahoo.com	$2a$11$yJWGV2ipmmiA9GuTKTEQBuQ54WjqYVZ4ARXPb3ySBm/HtcHCHq80i	Esteban40	2025-09-09 22:01:14.559449+00	59	CLIENT	0199307f-5159-7992-9af9-4c74da78817b	ENABLED
Krista_Witting15@gmail.com	$2a$11$mUuRApE58l/yIn40uijQxenRIzF9TCWmFAGEU4dlkJUCUOJHguinO	Talon.Swift89	2025-09-09 22:01:14.909845+00	60	CLIENT	0199307f-529e-73e7-adef-8ff11aa63087	ENABLED
Boris.Parker@yahoo.com	$2a$11$6jM1KzbkL52l2EMZ1ZYcJeJA1wHLVT/rMQpNmRFqS.RBicxUbgAgy	Tessie.Walter98	2025-09-09 22:01:15.311404+00	61	CLIENT	0199307f-5437-7a9a-acd5-0a3c79ecc3e9	ENABLED
Charley52@hotmail.com	$2a$11$BUD3sl2YGoqiwUUML.3dHe6dbGPKdZkvA2k6g/oUbGo2mdNJtyB3G	Gabe_Weber	2025-09-09 22:01:15.606409+00	62	CLIENT	0199307f-5570-7ba7-9278-9337e9c7c5e2	ENABLED
Cordell2@hotmail.com	$2a$11$S5tYTkv048twAQ/6dpERLuLV5dBCA4BLyccvOQvMdi4ZUy8dFlO5y	Mollie.Kris24	2025-09-09 22:01:15.945561+00	63	CLIENT	0199307f-56bb-739d-b3a9-cbb860015d3f	ENABLED
Bradley33@yahoo.com	$2a$11$kYQ91hqB3iTsncxO1d09WeUo0gRkxEK2T4aiwUm2hUt4.RSJyyjLC	Madonna_Padberg	2025-09-09 22:01:16.267113+00	64	CLIENT	0199307f-57d9-7b40-9c16-6c8a4fd51049	ENABLED
Jordon76@yahoo.com	$2a$11$pKv.WjgD2eVAvFuOG7uc2ufjygwMjFApiSBK1bzhv4DMaRmU2Z0YG	Raven_Kovacek0	2025-09-09 22:01:16.592852+00	65	CLIENT	0199307f-5950-7659-8964-9940e4ba2b84	ENABLED
Ariane.Kilback39@yahoo.com	$2a$11$33aETGebcyGXrkqtdasnK.fxKjmv/XTTwLIjo3hQOiIgypilQMPj2	Jerrell.Durgan51	2025-09-09 22:01:16.910931+00	66	CLIENT	0199307f-5a85-7f28-95cc-31f7eea9d5d5	ENABLED
Schuyler98@yahoo.com	$2a$11$HtajljSoagEWNWVbbJ4youp4sU8L5hykakktChJD6QFl1HwVFqKKy	Quentin_Mraz	2025-09-09 22:01:17.303127+00	67	CLIENT	0199307f-5bc8-7355-826f-c69702402e03	ENABLED
Carson_Blick@yahoo.com	$2a$11$VlcKvz64Vxg/trKxHmMZkuyQKT8a0UyER.s.Hot9CwbkrbMTIhaPK	Connie.Schulist13	2025-09-09 22:01:17.62817+00	68	CLIENT	0199307f-5d44-77df-b9f9-718b264ae4d3	ENABLED
Maurice52@hotmail.com	$2a$11$VxkQoe.B5yza0Woe4mVQwOGCEKv6Exy/pMJ1i126ou/4VnEEmdgEa	Polly.Conroy	2025-09-09 22:01:17.944488+00	69	CLIENT	0199307f-5ea3-7546-b0cc-a3e3d2172047	ENABLED
Virgie19@gmail.com	$2a$11$MOTLcULxRfB1zTfN6ddYzOI6tNRNRRT02bqeAlW0bQybJF/2iqixO	Elbert.Hamill	2025-09-09 22:01:18.295723+00	70	CLIENT	0199307f-5fb9-72f6-be69-847dd382f504	ENABLED
Paris.Runte@hotmail.com	$2a$11$uKVTn6htrob7W6T6HrLGm.WGuiQ1WwH6ULr52/ubEtx3XYajVGlOa	Dean.Wunsch	2025-09-09 22:01:18.602468+00	71	CLIENT	0199307f-612b-7ca2-9768-b697610741d7	ENABLED
Hans71@yahoo.com	$2a$11$dtriu6/C5WNqNiaq5S5NLeQFZfQq1heEyqAJ35yDPiWZ4G8iD6HMq	Savion_Braun98	2025-09-09 22:01:18.915614+00	72	CLIENT	0199307f-6254-7e70-9de1-c7d18dff0553	ENABLED
Amelie_Volkman@yahoo.com	$2a$11$nLK8JLdgw9TP/UM6QcAY6O3JRY1x1m.xE9PiHHYp6qY9Ne25LoCTO	Rebecca99	2025-09-09 22:01:19.302113+00	73	CLIENT	0199307f-6374-7827-bc39-d8e7a10ab3cd	ENABLED
Ashtyn_Senger99@yahoo.com	$2a$11$RIWyquSy7It5qcgryCzwyeIP3qToQ9iuavLD4POKxAuId4IoFhNwu	Nayeli85	2025-09-09 22:01:19.65796+00	74	CLIENT	0199307f-6522-7b6e-97a5-8af15ace2353	ENABLED
Shawn_Graham40@yahoo.com	$2a$11$zaZFyODyD7XmCnBJ0jq9g.CLrMzndapnP2xwpNNfuZAVF2KRCBDc6	Perry29	2025-09-09 22:01:19.989519+00	75	CLIENT	0199307f-669e-7762-aeb3-17768ec87000	ENABLED
Giuseppe_Schimmel@gmail.com	$2a$11$8xN53UtZVBgt5xzSFnuWae1LhFg.mVvDzIoFo36bPOri6SFSKq7gS	Vivian65	2025-09-09 22:01:20.319506+00	76	CLIENT	0199307f-67bf-72ce-ab97-557dc7ba5de6	ENABLED
Lisette_Rath@yahoo.com	$2a$11$LNLyYuTzWdfj3ubvwjBs0OQHrOt3uS1HqApS0Q7JK4BC14Ej3HjW.	Vivienne_Weissnat	2025-09-09 22:01:20.633842+00	77	CLIENT	0199307f-690b-7a3e-bd62-1f07b907b5d0	ENABLED
Lilla11@gmail.com	$2a$11$huKkNJIpXPL/jRqHFyrD.Oc7Z8oSRmIPb1OlPTBDmBob2yJNsht2m	Kira66	2025-09-09 22:01:20.961211+00	78	CLIENT	0199307f-6a42-709a-8a46-2b712bdb069d	ENABLED
Jeremie_Wintheiser22@yahoo.com	$2a$11$OM3V13vu2lSskpjU6bqWoOw3zTxQL9R00rhVPgfAbQ94/S5dsxZKm	Carmelo_Hauck54	2025-09-09 22:01:21.324945+00	79	CLIENT	0199307f-6ba9-760a-b064-daa5ad4e523f	ENABLED
Beatrice_Hintz@yahoo.com	$2a$11$BDhO4QbVv5elC2Uw.oEBNuqPZAeQhV785gqoYw58BssX7xCyN2Jvq	Grant.Braun11	2025-09-09 22:01:21.640198+00	80	CLIENT	0199307f-6cea-7b27-b0cb-f47b01d7c1fe	ENABLED
Randall.Dibbert@gmail.com	$2a$11$ON0G3NqOmDZba1KmpebzuO5Wf4weHo94pZG4BcpdhjITlOGaPmJSi	Cathrine96	2025-09-09 22:01:21.968913+00	81	CLIENT	0199307f-6e58-71ab-9b90-d975b0a3c025	ENABLED
Braeden_Shields@yahoo.com	$2a$11$GH5OoMSEUgLmGubwTgMDvOG/8Y1EF6p3Lk6K2YuyZuLhEzxMzvcv.	Josefa80	2025-09-09 22:01:22.298364+00	82	CLIENT	0199307f-6f8d-766b-adaf-22d1ce96d479	ENABLED
Sienna.Quigley0@yahoo.com	$2a$11$JSFvuqTOYviud3hzDN6eg.zYlwk7S05NvUQOBgtiVyRULP7kMERhW	Paige_Champlin	2025-09-09 22:01:22.646323+00	83	CLIENT	0199307f-70f2-7c8d-b907-a2d17427adf9	ENABLED
Abby.Kuphal@gmail.com	$2a$11$aZvx/ZVw2aVPnF.YnybYfewwSd3AfmDDyK4uNPVkoxTJbY8GxvsVS	Lavinia62	2025-09-09 22:01:22.977151+00	84	CLIENT	0199307f-7226-7763-9129-0a742c795e2a	ENABLED
Perry.Purdy@gmail.com	$2a$11$ps6E/IicJpM3cJ/3PzE2WecqsigX5OJJNn88YIS5Ge03WrOpchoE.	Joany.Boehm55	2025-09-09 22:01:23.323764+00	85	CLIENT	0199307f-737e-75e4-8462-2da471a3fff4	ENABLED
Harvey.Batz54@hotmail.com	$2a$11$u.07r/YWkry8zpz613PA7.Krt9aY0SDBX5WR1XohpcOiPVSl.LpD.	Alisa.Thiel	2025-09-09 22:01:23.664367+00	86	CLIENT	0199307f-74cb-7d48-ab15-ab41d2550ac4	ENABLED
Jordan76@gmail.com	$2a$11$z.KHoDzQgRRm2bOcCAu9Ee0H58mKECz6vWc1H9KScjW9tGK70aj6C	Alice.Weber82	2025-09-09 22:01:23.978672+00	87	CLIENT	0199307f-7639-7716-9ddd-685fb524373f	ENABLED
Nia_Weissnat91@hotmail.com	$2a$11$TNB4qm5ry.gOvozAJgteoufVlLh4ZzkTHA48LH.deBQUGgfb9Rixe	Tianna88	2025-09-09 22:01:24.317373+00	88	CLIENT	0199307f-7764-7f16-b419-8f66e57a631a	ENABLED
Karolann_Stokes@gmail.com	$2a$11$MR7t2dFAo02gJw9LL.QUn.Rm1kVBNGVnHllLNLuSXuMILjdsf5Io6	Karelle_Roob	2025-09-09 22:01:24.767291+00	89	CLIENT	0199307f-78bc-7019-8a91-2eaba022c59d	ENABLED
Adrienne_Dickinson@gmail.com	$2a$11$XrSykAxcKzn5zBj3Fkc9h.tAba2YNR5iY6BP2YkfEP8ot/kNHGXNm	Alycia_Hodkiewicz	2025-09-09 22:01:25.027325+00	90	CLIENT	0199307f-7a66-7a87-be1c-df439634af57	ENABLED
Brooke_Wuckert5@gmail.com	$2a$11$mnH5Qw8icCPJthobvwqqVuEC0rt8pzZ3NasLXLNBvjSX0Vl/QZq5y	Gus.Jacobson22	2025-09-09 22:01:25.444213+00	91	CLIENT	0199307f-7bea-7758-9259-7d40461d61e4	ENABLED
Kenyatta_Skiles3@gmail.com	$2a$11$sABIXi.KEzmkP3zV0yIxAOxgLjVChRCu2MT5pZgtNT8D5GDECH82u	Alene49	2025-09-09 22:01:25.755093+00	92	CLIENT	0199307f-7d0f-7a9f-9eb6-ccd3df03ea70	ENABLED
Emmanuel_Aufderhar@gmail.com	$2a$11$noj1.1ELrpReLn9h1lbpmebDO8OJlUBbwlK/joP48BwDi.C8TGZTu	Soledad.Little	2025-09-09 22:01:26.11321+00	93	CLIENT	0199307f-7e73-759a-8432-d921a986abce	ENABLED
Justen_Koss@gmail.com	$2a$11$4gejeycG2YveZRbDYrsLIOSmEb.X8HkyRY/cr/x1IqDg.mVkC1j5e	Nicolas21	2025-09-09 22:01:26.447643+00	94	CLIENT	0199307f-7fa9-763e-91f5-fe45d478631e	ENABLED
Lauretta67@hotmail.com	$2a$11$9hTvm/BRzTf8W9yrgwQ.E.efGvlRe8ngj66tScp6EAkqPuo8dBUwq	Akeem22	2025-09-09 22:01:26.773436+00	95	CLIENT	0199307f-80ef-7f5e-becc-d3f62d0175ab	ENABLED
Forrest_Kautzer62@gmail.com	$2a$11$BTCOKXPaM7hLYARfj8aQsO4.pHbwm1QUHjHVleo0hp3xQ5.FCdaHG	Christian34	2025-09-09 22:01:27.034715+00	96	CLIENT	0199307f-8239-7757-9de2-d47f9302d66e	ENABLED
Erika_Schmeler@yahoo.com	$2a$11$9r7IT6bRJRySEnqD7kzAeuJtLOB0LwdX/R4Rzod2d4ZEFDCwgmpr2	Gus.Zemlak79	2025-09-09 22:01:27.507897+00	97	CLIENT	0199307f-83d0-76a6-85cf-6563ea11b957	ENABLED
Queenie34@yahoo.com	$2a$11$gNMBgkvCU5NjTjTJVF/jyuDmU/mMBdQUwvLJH7uspEipfrTN1YOJ.	Reyna_Bosco53	2025-09-09 22:01:27.845974+00	98	CLIENT	0199307f-8525-7ae4-a9ef-27dc9bb8513b	ENABLED
Alessia64@yahoo.com	$2a$11$B4EdKLr9FRCYc0Gi7SsfBudGmEPVcF21bsOdVulrQVIbhpWMRPJFO	Lesley.Dietrich	2025-09-09 22:01:28.133452+00	99	CLIENT	0199307f-8666-7ebd-9c8e-78500755a0a4	ENABLED
Danny.Borer73@hotmail.com	$2a$11$kOVAAZ2g6TIBtlcy57vCh.25DHRV1YmBCxSNQbU88WL2cMZzonm3y	Georgianna.Mann72	2025-09-09 22:01:28.510947+00	100	CLIENT	0199307f-87a4-7b91-9b71-67b6755f6b03	ENABLED
Daija.Kilback95@gmail.com	$2a$11$tKwzyy2zzfsfdNLlbHomIushava/xV//5uWnTIrIkubI4V6w2kuU6	Kristy95	2025-09-09 22:01:28.832006+00	101	CLIENT	0199307f-8914-7fee-878b-8ea71c75c917	ENABLED
Tristin_Abernathy48@gmail.com	$2a$11$fa8dZesIR59qhH/5jZ2uNOdT8mKK31FKPyVxhdSyrcgP2K5zmFUWS	Earlene.Von61	2025-09-09 22:01:29.165376+00	102	CLIENT	0199307f-8a3d-70bb-962a-9f7988fd2b63	ENABLED
Pierre.Reichel87@gmail.com	$2a$11$beUtksbyY.fbPbkOPCUhoeYKiw0rksuFCfXgzXgLY0mTlXRjc.8SW	Cecelia_Bauch10	2025-09-09 22:01:29.509013+00	103	CLIENT	0199307f-8bbd-7d36-abb9-11e620af438d	ENABLED
Milford.Kuphal34@yahoo.com	$2a$11$5rL8H6aFJnC8F87Si3uhvuz2XQ0goL5hbcdRB4VVQo8MHxNchVkcW	Brian25	2025-09-09 22:01:29.846113+00	104	CLIENT	0199307f-8ce1-7dea-959f-70efe27cef65	ENABLED
Micheal_Effertz65@hotmail.com	$2a$11$4Wd7LB9nRasrOAkqxzuee.RN8VQWClzku1ebelMyvcQ.VHZDaT8Kq	Beverly.OReilly	2025-09-09 22:01:30.191508+00	105	CLIENT	0199307f-8e5d-79ce-9ee4-81f4dc00320d	ENABLED
Adriana_Corkery@hotmail.com	$2a$11$L9QTYW.TU26Xc/W1d.v/IuoYjYFGb/sHg9bv.TkwwP/zWYA.dLRxm	Candice22	2025-09-09 22:01:30.516987+00	106	CLIENT	0199307f-8f9f-7b1b-8d92-c13274c863fc	ENABLED
Robbie_Towne@gmail.com	$2a$11$wMlqiKRVmNatmQTSgYuJc.uWXvMpzcS7hXqGPUlzjzLzYJBsdSs5K	Iliana.Reilly82	2025-09-09 22:01:30.849043+00	107	CLIENT	0199307f-90f4-7253-9c46-3ac0f28ae7ef	ENABLED
Lizeth76@yahoo.com	$2a$11$PGwiwjCEM770IrDUUcGfIOhtdGatKEH3Ivhq2NzP/U6DWiQKuHN1C	Wilmer83	2025-09-09 22:01:31.199026+00	108	CLIENT	0199307f-922f-7a65-bb1a-28082569fc5d	ENABLED
Americo_Gulgowski@hotmail.com	$2a$11$3Gt2.5Iifqiafs//FuJU8.jIn9u.8pXcDZUk0/Yt65FQlOw8U.acy	Ozella_Wiegand75	2025-09-09 22:01:31.549238+00	109	CLIENT	0199307f-93b2-7ee9-8369-02417a82a865	ENABLED
Alfred60@gmail.com	$2a$11$5VcGnGr65C/qS0/J9ww5qu5hKdMtz/6.hdMgshcDql9PLFUdi2ore	Pascale.Smith	2025-09-09 22:01:32.120209+00	110	CLIENT	0199307f-951e-7b80-b1c5-c8ce1e0185e5	ENABLED
Francis.Krajcik25@yahoo.com	$2a$11$RmgmNR7pZR5bnL45lfqgTu/2rrfu.Xyob4QGWCR0tuH7wQS2FEMyG	Forrest.Mraz	2025-09-09 22:01:32.493101+00	111	CLIENT	0199307f-9776-7184-bac1-3f03fb4ec44f	ENABLED
Elsie.Torp49@hotmail.com	$2a$11$LFqmWzeKHPZzi7ntesQkseP0IzV3652G9hYcDA/N4WIIrCwWxDHNe	Fritz.Hilll72	2025-09-09 22:01:32.961261+00	112	CLIENT	0199307f-98b4-7a5c-8980-c0fb193cc7be	ENABLED
Lina40@hotmail.com	$2a$11$yuWpSWq4RzCxPSpP84Ht8.O84TC8VHbGr27VWJORL7P6/TdC2TUje	Jack_Runolfsdottir	2025-09-09 22:01:33.279035+00	113	CLIENT	0199307f-9a6a-74b1-9056-9d07a2a99817	ENABLED
Emiliano_Kuhlman@gmail.com	$2a$11$i4qO9mnWnsP5XUTKc.fr6.lxhZcbZoE4JnlhE.5tr6lTN.DyHGFgi	Toney.Keeling6	2025-09-09 22:01:33.707414+00	114	CLIENT	0199307f-9bb1-7c57-9b19-60b03887f378	ENABLED
Ofelia.Paucek21@hotmail.com	$2a$11$UlGrubuhGGt2HKXroBmqM.kHZ05ktHryqoTGXybxP0kYneqsbl63C	Deangelo_Treutel56	2025-09-09 22:01:34.005077+00	115	CLIENT	0199307f-9d57-7355-9a11-3acc0bc86303	ENABLED
Ansel_West34@yahoo.com	$2a$11$V5Lo1WSSvsT2RVxRD3I/u.NLBPKP8S0Axzmf8FO1O4HRwIfAp9POm	Shane.Anderson5	2025-09-09 22:01:34.348506+00	116	CLIENT	0199307f-9e86-7d5e-a321-a8451f411496	ENABLED
Janae.Adams66@gmail.com	$2a$11$gNHBAu/b9yXo3dsYkyzt4.RRE/SEGtw0SdXM9ml/FVSY7xEd1XGh2	Garth85	2025-09-09 22:01:34.75327+00	117	CLIENT	0199307f-9fee-7640-a344-5054d4a999b9	ENABLED
Hanna4@hotmail.com	$2a$11$SdxiXwjUR2W9Z1fjSF52he/pKtv1OBvWzixJwpaA7pF5aNZAhf3km	Elena18	2025-09-09 22:01:35.115791+00	118	CLIENT	0199307f-a18d-7b86-bc56-8fb76cf57a31	ENABLED
Raphaelle_Dach@gmail.com	$2a$11$3xlTInauEhLeK0wnDh..zuznSbMCf5F/W6rYQj4wyX8t6Vc.4RatK	Richmond43	2025-09-09 22:01:35.43382+00	119	CLIENT	0199307f-a2d3-7e57-a0e0-a718603cfcce	ENABLED
Hailie_Jerde14@yahoo.com	$2a$11$RFUknIk3vWwa4MaFLM7ug.6PNJVuu3pecXizOt/GE904I2Bw8nEUK	Jazmyn.Rogahn53	2025-09-09 22:01:35.886596+00	120	CLIENT	0199307f-a453-782a-b4bb-c2bc28c9d991	ENABLED
Annamae.Ritchie80@yahoo.com	$2a$11$WaFYOHB8JeR1wEV4xx1RRul4StwGLsyb4LOwvZLTlGgAgE7zL1R.K	Reggie_Legros74	2025-09-09 22:01:36.237005+00	121	CLIENT	0199307f-a5f3-747a-8199-cd1e3495cf44	ENABLED
Karley.Reilly42@yahoo.com	$2a$11$4X7llXbw52f18EsXQTCP9eXiODf1Ph6kRNBLwGi/vMYltlw0Z0/ua	Shanie.Tremblay66	2025-09-09 22:01:36.575983+00	122	CLIENT	0199307f-a730-7acf-8e5b-430acfd58ada	ENABLED
Adela.Huel71@gmail.com	$2a$11$GJq/P1cCklZDmY3sEg.LkedCL1rUoalJkNDyYw2qp/zMbuat5MJAS	Waldo89	2025-09-09 22:01:36.934488+00	123	CLIENT	0199307f-a8b9-798e-8fb8-d0354d170ea9	ENABLED
Bettie.Smitham@hotmail.com	$2a$11$qnzZC9r3omZS.hEO.Es7QeJxl6JYwbuQYv.d0d4we5.eqOIuwWA2e	Josiane14	2025-09-09 22:01:37.325618+00	124	CLIENT	0199307f-a9ea-7fca-8fd1-a729757d0bd5	ENABLED
Daija62@yahoo.com	$2a$11$PO6eMJ557RMMPEW6ahLNC.IuQ5A4i3kgFTHBvL/fJapnJ/nfGmPMK	Marco.Yost17	2025-09-09 22:01:37.649451+00	125	CLIENT	0199307f-ab89-7246-ba6d-4ae9224d4a7d	ENABLED
Newell_Conn@gmail.com	$2a$11$Poo2tkX2cLLL1l1ZikP1geO/IryDcOe1T/0u.WnZ75rbjD0C5ycg2	Orrin_Gorczany	2025-09-09 22:01:37.999526+00	126	CLIENT	0199307f-accd-71f4-ad55-a635bc41595c	ENABLED
Arlie.Cummerata@gmail.com	$2a$11$E4sna6LcuIJwT1VmARaF3uhUjE9cX5NRsntxX.6ueA1E9vwOCPyCi	Aisha_Feil46	2025-09-09 22:01:38.367903+00	127	CLIENT	0199307f-ae4d-75e2-81e5-b9d89b852540	ENABLED
Stacy_Dietrich59@gmail.com	$2a$11$eph9OIwTHM0gBHGcWuxX1.XC..eT4D9.6jR7Ibf6BxT0JqfIBz8g.	Billie16	2025-09-09 22:01:38.661099+00	128	CLIENT	0199307f-af8d-77eb-9235-36e034abdaa6	ENABLED
Verna.Green58@hotmail.com	$2a$11$jP2F22khFPJDWKhrJaRFdeFc8D.Tw6wNrfDbOiOnt20gRsVF90PmW	Judson37	2025-09-09 22:01:38.992496+00	129	CLIENT	0199307f-b09c-7a09-a4ee-e15c21576e9b	ENABLED
Retta_Conn93@gmail.com	$2a$11$7vQJl7J2rUOnF08xtEFa8e96h7S.FEJj5glh4vUKqGI53Um2nXuvG	Rowland_Dare	2025-09-09 22:01:39.364561+00	130	CLIENT	0199307f-b236-7a11-8990-69b68a901379	ENABLED
Lyla.Beatty@gmail.com	$2a$11$kfYlYJWJduxRwliHK97RkOm/NRTHrSq3Z4gtz8ebXKK2dnqJrKVTC	Herbert42	2025-09-09 22:01:39.747686+00	131	CLIENT	0199307f-b367-72c3-babd-3a579439943f	ENABLED
Darrick66@hotmail.com	$2a$11$QaF.8d6KdPUDXo2E1okTVuG.uVmgB.Bi5VmSG3J1VKHMyskeAdTA.	Frankie.Johnston72	2025-09-09 22:01:40.164026+00	132	CLIENT	0199307f-b529-7b84-bc9b-d66b04ddfc00	ENABLED
Charity.Quitzon@hotmail.com	$2a$11$fWy3utCwMatZ.IskZyF9nOgwaho9hp7PWtzWw3tLfVjvX8TSwtvX2	Kenny_Halvorson68	2025-09-09 22:01:40.535919+00	133	CLIENT	0199307f-b6c9-74eb-a76f-b4893f877f5c	ENABLED
Willard95@hotmail.com	$2a$11$tH6bkuSj3RZ8Ke.5Q3UG7uMxA6B8cS8IprMVPpPt.FpR1DPov3yNq	Abbie.Cole78	2025-09-09 22:01:40.955273+00	134	CLIENT	0199307f-b81a-7648-bab8-0e71f7f172ee	ENABLED
Bernadine3@gmail.com	$2a$11$vxy7vYttlE7OI5gveLfXn.a.QEEBymvNLGigXSwrIfieDXy2HKBwi	Magnolia_Welch3	2025-09-09 22:01:41.274806+00	135	CLIENT	0199307f-b9b2-7558-829c-f442e76b3d69	ENABLED
Marguerite14@gmail.com	$2a$11$VOwnlU8kInGrUzbH4LfBg.UojyHMs6iesngMToM8iUdUN5YaIMqmW	Gilda.Gusikowski	2025-09-09 22:01:41.637665+00	136	CLIENT	0199307f-baed-733b-9162-5f81287b81b4	ENABLED
Otto69@gmail.com	$2a$11$2y01K.tU7FMn3q3iRR1MEOKknJ4t8no1/2CBj8FJGcVInZh3F6M72	Newton_Zemlak5	2025-09-09 22:01:41.981897+00	137	CLIENT	0199307f-bc7d-790c-8f7a-8dcf532c0206	ENABLED
Colten.Hintz@yahoo.com	$2a$11$WjTznFbeKxTSjrNvzKmkH.LE6B6ACsZ9BQZ.L/nxg/0q451if5NPu	Albertha10	2025-09-09 22:01:42.317624+00	138	CLIENT	0199307f-bdb0-71b3-8ae2-d9a1ec69276a	ENABLED
Shad_Lebsack@yahoo.com	$2a$11$nfG5.8kTlPSQD2jtO.PyceMakVYT2VHOH8cZV8.C7LpwxKAdRvYm.	Garrison89	2025-09-09 22:01:42.670487+00	139	CLIENT	0199307f-bf0a-7e93-8415-fec09a398e25	ENABLED
Ericka.Cassin@yahoo.com	$2a$11$g56sOMfbSef.7V45mKRftOVwZT4fU6BhiJlRqxY6oTGYcZzp5IP5C	Juvenal_Cruickshank83	2025-09-09 22:01:43.02131+00	140	CLIENT	0199307f-c065-70e0-882f-44a9fdf6f05d	ENABLED
Marianne.Breitenberg@hotmail.com	$2a$11$kaACGcLAmishBmhlQrsw0uy3SK5H6RR5PZHyTrFzv5OiDimLaygJi	Demond69	2025-09-09 22:01:43.381325+00	141	CLIENT	0199307f-c1fa-75ab-87b4-f5dcaeb86be0	ENABLED
Lenora88@hotmail.com	$2a$11$GYlmIejmVt8QF4e4F3cOveI2U15u2ZfVAu0uDrs3fQGGrQ8.ZFxIK	Salvatore.Schmitt38	2025-09-09 22:01:43.714923+00	142	CLIENT	0199307f-c32b-7a6a-8c5b-67cc8abe8f12	ENABLED
Darrell_Gottlieb27@gmail.com	$2a$11$c8NYaCGzMM4b1TOetFGZ4ODXV5adNHQ1dIP5yZLGTn2W9m54qHGyG	Kasandra.Wyman	2025-09-09 22:01:44.0269+00	143	CLIENT	0199307f-c480-71c0-b6c8-bc58105af822	ENABLED
Richmond30@hotmail.com	$2a$11$pRR0PjatybVxr5VR3AVJjOz9mIUSYrM3xIQpNG9CsEkW0fJr01YEO	Lucy30	2025-09-09 22:01:44.362879+00	144	CLIENT	0199307f-c5b1-7dc9-bf67-a445968bc68e	ENABLED
Chasity_Quitzon24@gmail.com	$2a$11$xAoboH5b/f4qedToRt1XKuaEXEXfQEsAdhE.dIxfdIY/NyyfOJLM2	Vito_OConner	2025-09-09 22:01:44.780938+00	145	CLIENT	0199307f-c6dc-7798-9f7f-5cc87229dbcb	ENABLED
Damien53@yahoo.com	$2a$11$zux5Y0UmCuyOlYncR5iNGeNbnazIiZpfwX7mNrS0WW.iY3VXVxsnu	Ocie_Lynch	2025-09-09 22:01:45.157261+00	146	CLIENT	0199307f-c8b2-7b8e-9a2e-df867435a798	ENABLED
Kiel.Schaden40@yahoo.com	$2a$11$l1fEzB.GYG3x1N43DTmcBug8.1XdkMiV.vBWAIBvDHnLhA8GKzuBa	Maximo47	2025-09-09 22:01:45.518894+00	147	CLIENT	0199307f-ca41-7ea2-9ef2-631008854dda	ENABLED
Arvel.Klocko@gmail.com	$2a$11$dzA.Fiy8F/GJD3hPiMH2HeqfSxVxT9vBHoDkoF0y1WK0AiBVkc00e	Adolf_Runte	2025-09-09 22:01:46.08005+00	148	CLIENT	0199307f-cc30-72d0-9238-0880113af03c	ENABLED
Zella.Rempel@hotmail.com	$2a$11$FXDXbH28zTR9pJywHG1fPONGovwpz35maFmG1HjCnAv/3pYptQfj.	Beau.Grady	2025-09-09 22:01:46.380849+00	149	CLIENT	0199307f-cdab-7fa2-9c61-db83d2c531fa	ENABLED
Zaria.Schumm83@hotmail.com	$2a$11$ASIw1zJgqQvwhdSfSg3Xmu0N0YckYkSCTrtFy3Cle3SKTn.5uKqaS	Stephanie92	2025-09-09 22:01:46.725628+00	150	CLIENT	0199307f-cede-76f1-a8d3-2eb8536e725b	ENABLED
Eve61@gmail.com	$2a$11$dArxh5PRUB.gIf7ZP5HxYe2vvNipY.X5ThbAYKTs.hAixs/8Q/N3S	Jamel79	2025-09-09 22:01:47.091692+00	151	CLIENT	0199307f-d044-7487-95e1-33f887a44012	ENABLED
Sarina.Mraz74@yahoo.com	$2a$11$YUrk7zHVW2x7ZVuZ7ID9BuG1/e7kv2Zv5vvkUGmMOzn8guGnM62x2	Nathan.Weissnat	2025-09-09 22:01:47.406257+00	152	CLIENT	0199307f-d19f-7cf3-930a-5eb197252090	ENABLED
Lucio.Stanton@hotmail.com	$2a$11$khUVBvhlNZaZO.jhyKHtXOzbSziBjKSQ21W2Q55EGIWotMrVxlW5e	Brandyn47	2025-09-09 22:01:47.757272+00	153	CLIENT	0199307f-d30a-7abd-a5da-ea32672be321	ENABLED
Lula.Yundt93@yahoo.com	$2a$11$dpw8KJesWU2xO91EmaNHoue/i0ML.3yzQsfmCq/F4S/OXtoOhBJoq	Nathaniel_Ferry7	2025-09-09 22:01:48.257714+00	154	CLIENT	0199307f-d459-7507-a83c-f9ecbd306200	ENABLED
Aiyana13@yahoo.com	$2a$11$GVtj6Y0vJZ8gllXU3Qe2gOQ7VCbDqyNOdbTMa/7a2M2hCQbOJ9xXS	Natasha87	2025-09-09 22:01:48.546842+00	155	CLIENT	0199307f-d618-7b5a-a0f8-1fc8eb861110	ENABLED
Luciano.Stiedemann67@gmail.com	$2a$11$CC.TCPouigkoNv0eJv9QWuNOXlLQQgBan4DR91Yr8roe4zQPQnEcW	Carlie.Moen8	2025-09-09 22:01:48.932919+00	156	CLIENT	0199307f-d75e-72be-834f-a29b95f125d5	ENABLED
Ezekiel_Nitzsche@hotmail.com	$2a$11$xtouQ1i2/dtr/Lba3QS3YuBIdWW5HJKZUnve.ZbFl.oc1o.0KOSQu	Coralie_Rolfson	2025-09-09 22:01:49.307463+00	157	CLIENT	0199307f-d8eb-7bd4-95ab-0e6bc30b9fee	ENABLED
Rupert.Kilback@yahoo.com	$2a$11$EMYE6dnUQJXF8Rl4Fam6Q.gsWsaa3U5FOJcSSYSKxh0HEaAxpUriu	Dandre.Ferry	2025-09-09 22:01:49.698065+00	158	CLIENT	0199307f-da71-7787-8987-5defcfcec061	ENABLED
Chadd.Schuster39@yahoo.com	$2a$11$E7PDmMG9PBSaAl5zrmfMVuauuxSz9QVcjGetHbxQsEBW1VtgSA/v.	Horace_Sipes50	2025-09-09 22:01:50.046162+00	159	CLIENT	0199307f-dbe0-7dc4-9011-c0cc9fd19221	ENABLED
Eulah.Donnelly24@gmail.com	$2a$11$Do3ZrQC.QjP0hu4oZQiPV.747RjkqnyTqliH/cy8W08j3Sy2cTmze	Arno_Yost81	2025-09-09 22:01:50.39176+00	160	CLIENT	0199307f-dd5a-7b04-b9ef-dbb434ba45d8	ENABLED
Charley.Legros66@gmail.com	$2a$11$sJeuwBbvZWyPXD.9cWlz6e1gjtg7s4XXAgMgot.AKLG0sUZzlabSC	Christy38	2025-09-09 22:01:50.687549+00	161	CLIENT	0199307f-de79-74f0-8a77-2fddaee0f778	ENABLED
Abdiel_Hoeger43@yahoo.com	$2a$11$qYMVz4SNSGce9zoGfCIES.UFFkjV5TzBeNFW8IaBy3jkv3MRKnj0O	Meda_Hettinger	2025-09-09 22:01:51.086651+00	162	CLIENT	0199307f-dfcd-761c-a587-5ac5ad03a346	ENABLED
Cara.Brown@gmail.com	$2a$11$lTGUNGfc.N72hHVi0f5Wd.UNEPQoBmWablnmVOBpPJufI/.A5CRca	Jodie_Tromp7	2025-09-09 22:01:51.40142+00	163	CLIENT	0199307f-e121-7b99-9309-1da4ba9bd074	ENABLED
Jedidiah.Gorczany12@yahoo.com	$2a$11$ruFr4ZGJmh455Wig5wwDMeFbRZFzfdI1YFXZud4qxyi2jS3ydUUEq	Marlen.OHara35	2025-09-09 22:01:51.81139+00	164	CLIENT	0199307f-e26f-789f-88f0-5b5bfa4b571e	ENABLED
Bobbie_Baumbach@hotmail.com	$2a$11$MRDQfcDv.REDPc6NwKL3hellhDy/bl1YfTNFWKN5stDkKYXdQ0xJm	Damon_Marks55	2025-09-09 22:01:52.178156+00	165	CLIENT	0199307f-e428-718b-8cb0-508e9f073e26	ENABLED
Noble28@yahoo.com	$2a$11$FOGpanhORPDHJcoXPxxG9uXNa/3.GDQVa23ansXuwLN5BLLUx.dvC	Sheldon6	2025-09-09 22:01:52.526325+00	166	CLIENT	0199307f-e5a3-7076-bff6-6a190a6dacd2	ENABLED
Jedediah.Ernser@hotmail.com	$2a$11$JtS/LsMScBvIu3kcbey0Eeq5Af8Iwg85WujaZRNgEvbdXLGJt8yQ.	Arturo.Miller	2025-09-09 22:01:52.909666+00	167	CLIENT	0199307f-e6d7-7ca0-8382-523b1617f189	ENABLED
Sarah.Wintheiser55@hotmail.com	$2a$11$o3LGDvd/nkkk.YBLMjv.Pent7ehBRQTZ7oCtPPTkH9YLV3Lq8K6Gm	Claudie55	2025-09-09 22:01:53.238288+00	168	CLIENT	0199307f-e873-7877-8951-c784761188ad	ENABLED
Katheryn88@yahoo.com	$2a$11$ohuHCwfmcB9H2VHD1r.3d./19dubC5E3PVB7aGDQa3bYUTw0RYCLi	Maryse.Harvey	2025-09-09 22:01:53.618454+00	169	CLIENT	0199307f-e9b6-7d7d-a5e9-e80bda369157	ENABLED
Rahul.Wolff@gmail.com	$2a$11$sfvo/WKUdywj4cW1Yts/quWm4ZxSdYk3Hg9LrRUJGbsnr2Wi3BRm2	Felton.Gleichner	2025-09-09 22:01:53.989679+00	170	CLIENT	0199307f-eb5e-7a1a-add8-18f879bfbba9	ENABLED
Akeem2@gmail.com	$2a$11$5QO0na5Z9ec7n8hWtynU1e5RTS3Gi9TvOXYTXL62dwK8OHI5P1q1y	Kay.Kassulke	2025-09-09 22:01:54.360762+00	171	CLIENT	0199307f-ecb0-76b5-a742-107923a2cf22	ENABLED
Agustina31@hotmail.com	$2a$11$YrU0pSmtoNinSj9z/I/JHeWehI1huTGAIRFUWErs1ZRFb93iSR7n6	Frederik_Pfeffer	2025-09-09 22:01:54.789424+00	172	CLIENT	0199307f-ee3a-7f06-92b7-96c235f20d5f	ENABLED
Adalberto.Luettgen84@yahoo.com	$2a$11$izd97rSb6iilbl28Rb0kEOd.0R/7AEEex.8GCxc0mBLrq3cTjA2jq	Lourdes.Rutherford	2025-09-09 22:01:55.157893+00	173	CLIENT	0199307f-efd4-7e25-9e92-921ccdb24803	ENABLED
Derick.Stiedemann@yahoo.com	$2a$11$e94Gf06dNsZFAHqD97MrnumQzPvv7QbOY/6SbMAvDWJQnVvEhP1VG	Marta_Douglas70	2025-09-09 22:01:55.515796+00	174	CLIENT	0199307f-f151-7100-919f-93ac9e153dab	ENABLED
Waylon32@gmail.com	$2a$11$dhNyVEPrlNaHChOlqz9Iq.HLV/7Lvw1mazFQYMHU0qdqLEVQ36NeS	Kaitlyn73	2025-09-09 22:01:55.895282+00	175	CLIENT	0199307f-f2a0-74f4-bce0-70c22d3acd22	ENABLED
Claudie68@gmail.com	$2a$11$WomELvECotDpLE6b.efi0uHoSffBPPGSWo1t2q8BT2OHB9pRIUAlC	Alverta.Satterfield19	2025-09-09 22:01:56.211678+00	176	CLIENT	0199307f-f401-7cd7-aacc-2c75783b88b4	ENABLED
Pearl38@yahoo.com	$2a$11$GomiapzUbTNXJzn9aFtfFuNiZRnBF2VgsL1mmZeb2NppTBCk9/Ev.	Blanche56	2025-09-09 22:01:56.547445+00	177	CLIENT	0199307f-f545-756c-9b07-102772d4a94d	ENABLED
Alexandrea.Zulauf@gmail.com	$2a$11$UngY.PwjgaUtDs7rzCGMc.6VBQEFjwuMyKIuLbVa8Xe72RYZ0mjxy	Adrien.Skiles13	2025-09-09 22:01:56.90572+00	178	CLIENT	0199307f-f6ba-70ea-ae49-6f2a50f6d3b9	ENABLED
Hyman0@yahoo.com	$2a$11$TQbdAFegINzwqQISQoGYteHrMwgXg1idaoGH6zrDGxsAKcW11vCSe	Xavier6	2025-09-09 22:01:57.234734+00	179	CLIENT	0199307f-f7f3-755e-9827-3cc1b78f361c	ENABLED
June.Beatty@yahoo.com	$2a$11$4DKjEkrvSn01WZG2ZLwe7.dbAPXVzlQUxNokJYJPuLO1DyJww4ehW	Violet35	2025-09-09 22:01:57.578693+00	180	CLIENT	0199307f-f975-77f8-8293-bb3169d6a592	ENABLED
Hobart0@hotmail.com	$2a$11$bi2HDPjzE5ePkFrXypGdfugIq0bkExYbJOl/jhRxIAhtRinsm4CBO	Vella.Botsford	2025-09-09 22:01:57.892895+00	181	CLIENT	0199307f-faa1-73c8-95be-f271fea2517e	ENABLED
Stacey_Kshlerin25@gmail.com	$2a$11$ZVjulby5mKpJS526haMLz.wO94k4lnyjmUljdcTvsfZ8curzOMwy.	Scot_Beer28	2025-09-09 22:01:58.266462+00	182	CLIENT	0199307f-fbd3-7ce8-903f-b44fb595992a	ENABLED
Helena42@gmail.com	$2a$11$YVgC0v64a5KKQQc69t883upmbOFmlga4HvyD4juX377AY3OX4TyZu	Grayce53	2025-09-09 22:01:58.618348+00	183	CLIENT	0199307f-fd58-7c9c-a668-0b4490383165	ENABLED
Keyon.Ondricka42@yahoo.com	$2a$11$3GiQ5axs5GfiH1KZPS6uRuQdAUbCPvlLShliq0YsgeoDIMDkBWAxa	Elvis.Sipes	2025-09-09 22:01:58.975957+00	184	CLIENT	0199307f-feda-7ca8-b4c2-2d71b308e899	ENABLED
Tyreek.Murphy@hotmail.com	$2a$11$15/lyUh.KpZ0zYpW9tZ4hexswdIX619g7FiBALWJN466Elh2VY1Zm	Xzavier5	2025-09-09 22:01:59.369956+00	185	CLIENT	01993080-0041-7eec-8726-051b4ef3455f	ENABLED
Fausto_Russel@yahoo.com	$2a$11$6Ui3lrKJNOtODjtSGVM7duu2DZduMuDrSRg.5Tjz6fsIkkazf5Tbq	Alessandro_Dickens	2025-09-09 22:01:59.780606+00	186	CLIENT	01993080-01bf-7202-924d-5b891a5c6f8b	ENABLED
Curtis12@hotmail.com	$2a$11$i5MxGlURZXZseSzIfya73u.//L3DK5PRVs/E7Ha7Y/vnMyktayjKa	Chaim74	2025-09-09 22:02:00.166335+00	187	CLIENT	01993080-0328-77f2-bfab-f88b7cdd0698	ENABLED
Jermey.Altenwerth14@hotmail.com	$2a$11$hEsREfkeaT0DysqSTf7CReBuSmlsWrn1DfrZoKukAuwC.W16egYj.	Joshua_Wisozk26	2025-09-09 22:02:00.523586+00	188	CLIENT	01993080-04d0-7a77-b705-5f8526cea041	ENABLED
Elian_MacGyver62@hotmail.com	$2a$11$0t.JrPhGljEMUG1KpiU6V.q4RqnxE0dGXZIIDXRawuxq769Htm2YS	Lina68	2025-09-09 22:02:00.945675+00	189	CLIENT	01993080-062a-7e3b-b1c3-15b4ba766464	ENABLED
Tiana.Armstrong55@hotmail.com	$2a$11$4pSwuN2khSXRBEZOiFOdo.neBVy.0/VolGnv2o2a933c2Kc6IAEbi	Amelia_Hoeger33	2025-09-09 22:02:01.293357+00	190	CLIENT	01993080-07c8-797e-9203-81bfe7c8b3e0	ENABLED
Tessie_Lockman83@yahoo.com	$2a$11$XbfJS3MpzoUTRFfM27ingey9uQz0eC8j4O0ZeTVR91cwBjrWtsD.K	Timmothy_Weber	2025-09-09 22:02:01.821154+00	191	CLIENT	01993080-0923-7feb-b0e5-44ff48a2847f	ENABLED
Steve_Miller@gmail.com	$2a$11$Uqi.Vc.l1HR7CS9lbbmRW.ZlCYkbPEV3N7Obsy2SNJ6Tn38gjgSk6	Filomena70	2025-09-09 22:02:02.22421+00	192	CLIENT	01993080-0b61-7c54-b301-ff6204d607f4	ENABLED
Rosie.West@yahoo.com	$2a$11$9pPhHwxewOqK2lgIbafTQevD8wt6Y6BLCv5EpwVkmjCxBeoUOhtsC	Sammy0	2025-09-09 22:02:02.636242+00	193	CLIENT	01993080-0cf8-7a2b-8b2e-be4f017931f8	ENABLED
Keshawn.Sporer@yahoo.com	$2a$11$ZncHA4b5hRiLv3CXRYh9ee2dXnSycfYlV4YEXdFzazRcHGbDKth5i	Amie64	2025-09-09 22:02:02.994987+00	194	CLIENT	01993080-0e71-7656-ab14-a290d616315b	ENABLED
Dee87@gmail.com	$2a$11$oC.nfX4aXPzKqld6wGvy9O2TxyZusr/2HYEbjbwx4bPSzY3y2ooR6	Lexi.Sporer	2025-09-09 22:02:03.454409+00	195	CLIENT	01993080-0fda-78f1-880b-bdd76baee5f5	ENABLED
Freda.Jacobs32@gmail.com	$2a$11$dLwoIWw5pKRH0k4Ndf7ad.0OXHapzbGFDmNxbPboDnNitfcjZNKfi	Justus1	2025-09-09 22:02:03.818925+00	196	CLIENT	01993080-11b7-7f64-b809-78f5542a7a31	ENABLED
Erika_Weber@hotmail.com	$2a$11$qvsNpCevsXrZbQdOKQ6aBuvIZjiMszfZUufq8GJ2ASkUiD9IPGZcq	Daphnee_Macejkovic66	2025-09-09 22:02:04.202579+00	197	CLIENT	01993080-1311-7e09-8d4b-46e121edf0c1	ENABLED
Arnoldo_Von98@yahoo.com	$2a$11$JpmCZYj.aIoiQkwTkE/od.ezNWRjouLPH86OgvyEQ9o/iHHuHvGcK	Gerald.Shields81	2025-09-09 22:02:04.595395+00	198	CLIENT	01993080-14b7-7939-b97c-923bab8f2c00	ENABLED
Magnus50@hotmail.com	$2a$11$PRJhn9ozil2l/LPgjTmEK.LMbWdGnn1QJtGMrypXuL4IFVpJ3oiK6	Drake_Olson	2025-09-09 22:02:04.953865+00	199	CLIENT	01993080-160b-7a93-a9b0-47004dcd01f3	ENABLED
Maci37@yahoo.com	$2a$11$Ngh.B7hVvnk5QYKP9Xh57.l4BkAB.cLfPP/z0s5PgDNlhjKXVo.n2	Sadye.Dare	2025-09-09 22:02:05.345937+00	200	CLIENT	01993080-17b7-7f0e-9145-f930bb2f02f8	ENABLED
Beau24@gmail.com	$2a$11$UCL4K0TuPMM.FUdefRFCHubs5CuurXV71hUN8AU7qZf5VOfl6PbBa	Vinnie50	2025-09-09 22:02:05.716435+00	201	CLIENT	01993080-1908-710b-9cf3-145f2409f365	ENABLED
Judd.Bode14@hotmail.com	$2a$11$tziaW/E47OVgaK5dSO3XSuFDr7y82yLaYWWe3iW8HtKeQxHOike8O	Kallie.OHara	2025-09-09 22:02:06.038501+00	202	CLIENT	01993080-1a73-7180-9b4d-f5151c2b6556	ENABLED
Assunta91@yahoo.com	$2a$11$Rft8.LHoylTGIB63lcOtA.dzTuYCBtIOIulXaAEVFCbSUV8TPE.Fi	Tierra_Little	2025-09-09 22:02:06.498084+00	203	CLIENT	01993080-1be9-70b8-805e-035e6119c8ee	ENABLED
Gust27@gmail.com	$2a$11$8ttRBTWAeaZFk4kaiv84NucNkehVnIoj/49ldBgoIe55PvEOdLovq	Oral_Berge44	2025-09-09 22:02:06.817332+00	204	CLIENT	01993080-1d80-7284-a0c7-13e97e2c5b77	ENABLED
Isidro.Beatty@gmail.com	$2a$11$VRPmW.4IPFg2aslLwPfBlOArRFTVTmrGPrGSS3maxe.A7x5iy9b8a	Dixie_McCullough	2025-09-09 22:02:07.146441+00	205	CLIENT	01993080-1ea8-72d5-802e-79f086d71fe2	ENABLED
Kelvin_Schmidt@hotmail.com	$2a$11$BDsb8yMLBcUQ5/1cat6VCOlYHd43ZkKF4baGdbeE1k9nT.7X9lY46	Maci_Dooley27	2025-09-09 22:02:07.572872+00	206	CLIENT	01993080-1ff8-7088-90d6-89044269c160	ENABLED
Nya59@yahoo.com	$2a$11$o0J0LPB0jHrDfn8lCh00L.D9kD5d4NsXFEIxh75SPYcBny8U98Hrq	Dimitri.Ondricka24	2025-09-09 22:02:07.867523+00	207	CLIENT	01993080-21a5-79dd-b280-0f412faeeaec	ENABLED
Giles_Predovic@hotmail.com	$2a$11$G6L8e2QG6OKyL33r2Y4HL.Dt1pOafwz7Cm7t44KMXjF7MwETr2.b6	Macey.Lubowitz	2025-09-09 22:02:08.23196+00	208	CLIENT	01993080-22da-73f6-9996-1cb5362d978d	ENABLED
Damian_Bahringer@gmail.com	$2a$11$k6Z8I6Vv61oJ0/O5BE6d7urZ58r0wBY1wU6fkWseGfht1.NIzT22u	Letitia_Hansen	2025-09-09 22:02:08.927535+00	209	CLIENT	01993080-248a-75c8-b03c-a41057b0899c	ENABLED
Elyssa73@yahoo.com	$2a$11$fnhrPZkQ2fvRqha2OUtzvOor4c6hW04WM7CS.HaBn5RQBDhtedfOe	Edison_Kris	2025-09-09 22:02:09.276519+00	210	CLIENT	01993080-26ef-7481-9daf-69f7c9060839	ENABLED
Spencer39@gmail.com	$2a$11$qXc1KA.HPTs83Gct8vxtROe7NQwvSDDZuMiJnStBQ2QvxrIrK6bH.	Emma67	2025-09-09 22:02:09.707356+00	211	CLIENT	01993080-2856-7680-8e7f-f2def6087fa5	ENABLED
Delphine.Prohaska41@yahoo.com	$2a$11$kDy5sGpFVtAAkERNVOLrZOYyleV1zXOiS292FoRPNe6/H4LwwfJDK	Esther_Hyatt52	2025-09-09 22:02:10.05778+00	212	CLIENT	01993080-2a1c-7f6e-8355-315aa8c9e0a3	ENABLED
Shaniya.Quigley@yahoo.com	$2a$11$.C0yMmQnLzGUsoZP95Ifh.U3zIr1q7YjqAODUrAzgupnKNZsfj8Za	Garnet44	2025-09-09 22:02:10.437876+00	213	CLIENT	01993080-2b49-7dc1-a918-e6bd8a2790d7	ENABLED
Deshawn26@yahoo.com	$2a$11$MMTYlw.2/2HgMo9IbJc5LOrZFRgAwcKzV1ih.24aGc6X88fVhu0he	Kira.Krajcik66	2025-09-09 22:02:10.813576+00	214	CLIENT	01993080-2d08-7abf-b55d-3826e6247263	ENABLED
Tyrel_Botsford@gmail.com	$2a$11$pAsTUGFOcIikojANHlNleudy/uuM8.Pd/zyAsgmr0t8ymXftkl.FW	Amina_Halvorson15	2025-08-23 00:34:41.623085+00	2	CLIENT	0198d459-5664-7cbf-8b8d-acb2e3c5fabf	ENABLED
angresdelavega@hotmail.com	$2a$11$T5DKVoMLsOizA.jO9dvyqevDuSRQfBgyvoZB0BRh/Rj3VwV/agLM2	andresvega	2025-09-10 23:37:08.095801+00	215	CLIENT	019935fd-7844-7989-b6bc-94747f573f85	ENABLED
jorgevergara@hotmail.com	$2a$11$1ifECM5Iq6FdcqhvfL206uHE8wr3X6SFItGyMMzrA5aWj0xnlh6yS	jorgevergara	2025-09-10 23:37:36.132798+00	216	CLIENT	019935fd-e5d9-7229-87e5-0722a8816a0e	ENABLED
ximenalopez@hotmail.com	$2a$11$z1tFNBUD3lVmnNxGqUNnHe52LSTnnX/Rsih7GyDtNDJJHkf.a1K2e	ximenalopez	2025-09-10 23:37:59.490769+00	217	CLIENT	019935fe-4123-7237-8abf-716791d136a3	ENABLED
joseantonio@hotmail.com	$2a$11$Ck5XDWSZgbcjy85DgS8fm.wyhCTic/6Xdc4KvzygOI1AlcQDswHQO	jose	2025-09-10 23:38:31.966684+00	218	ASSISTANT	019935fe-c001-7833-9e2e-eceb7cbbce17	ENABLED
gissel@hotmail.com	$2a$11$u21lZhFnyUNvjQEH88pX/u5.eV3L6/uYQAwA12MrG0OwD/cSdEf5y	gissel	2025-09-10 23:39:08.003017+00	219	ASSISTANT	019935ff-4c87-7d0c-ba70-4c85e5a0a003	ENABLED
antonioaguilar@hotmail.com	$2a$11$f/puOnbw0wCywSYRyZHUCOwm.oL8drCFlh7Sf0uqd9rH9uWv6Jt0u	antonioaguilar	2025-09-10 23:39:45.907098+00	220	ASSISTANT	019935ff-e0d2-7a36-8647-43ba9671d191	ENABLED
cliente@hotmail.com	$2a$11$6ERLqG9xfmdgmdGyK.DDgOdUqkpfKXRo4vKlr1kc/PeDqofEL3c..	cliente	2025-09-12 00:47:09.784142+00	221	CLIENT	01993b63-f11f-7d51-a013-a92607079340	ENABLED
\.


--
-- Data for Name: UserInformation; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."UserInformation" (name, phone_number, filepath, id_user) FROM stdin;
adrian	2389428923	\N	1
Violet Moore	556609954566	\N	2
Tommie Wuckert DVM	553360140685	\N	3
Ms. Megan Rice	556242099821	\N	4
Kent Beier	558523946754	\N	5
Dale Walker	555885810607	\N	6
Katie Johnston II	552316327431	\N	7
Jonathon Hand	556883335888	\N	8
Norman Cruickshank	553226370193	\N	9
Greg Franecki	558271005873	\N	10
Sadie Ondricka	556435346343	\N	11
tester	234905349053	\N	12
Vicky Jones	552686333165	\N	13
Clint Cruickshank	555211422148	\N	14
Cedric Murazik	555032496456	\N	15
Mr. Geraldine Luettgen	550728294346	\N	16
Winston Adams	553282449074	\N	17
Sonja Auer	550923979177	\N	18
Jeff Ernser	553471562118	\N	19
George Rice	558819317721	\N	20
Santos Runte Sr.	553981141888	\N	21
Patty Kovacek	552679291391	\N	22
Delia Witting	557658802669	\N	23
Ethel Cassin	555984889647	\N	24
Kelvin Schumm	559209658075	\N	25
Alfredo Huel	558013314799	\N	26
Jody Jakubowski	553201321377	\N	27
Douglas Kirlin	552406295744	\N	28
Gregory Wehner	558847460586	\N	29
Kelly Swaniawski	558150807358	\N	30
Candice Medhurst	556899492461	\N	31
Marion Williamson	552837241734	\N	32
Cary Lehner	555317222101	\N	33
Al Gaylord	550243853538	\N	34
Janice Boehm III	553759025679	\N	35
Julian Block	556395069162	\N	36
Felipe Weber	554071428056	\N	37
Inez Balistreri	551174001550	\N	38
Wendell Parisian	555126531663	\N	39
Kendra Kuhlman	551539609761	\N	40
Faith Reichel	552551714847	\N	41
Miss Steven MacGyver	554235687124	\N	42
Milton Predovic	551365290257	\N	43
Jose Kerluke DDS	557967755825	\N	44
Terry Welch IV	556965358696	\N	45
Nathan Cronin	558109428835	\N	46
Larry Gaylord	555562807638	\N	47
Enrique Roberts	552701969266	\N	48
Edward Gleason	550009485466	\N	49
Courtney Trantow	556835966205	\N	50
Ignacio Wilderman	550540482218	\N	51
Byron Gibson	556829133990	\N	52
Danny Grady	556583537665	\N	53
Dr. Bessie Farrell	559444616757	\N	54
Daryl Collier	553538912072	\N	55
Omar Conn	554233749804	\N	56
Francis Walsh	556003338232	\N	57
Billy Harber	552934140893	\N	58
Tanya Armstrong	558600297930	\N	59
Joanna Kihn MD	556315705972	\N	60
Miss Johnnie Champlin	550548779006	\N	61
Philip Dietrich DVM	554092164824	\N	62
Leigh Heidenreich	552839519049	\N	63
Mrs. Manuel Romaguera	552733901943	\N	64
Katherine Lockman	553872466081	\N	65
Lamar Waters	554115878290	\N	66
Marjorie Wiegand	552402373122	\N	67
Joan Metz	553671348057	\N	68
Nicholas Lesch I	558287091614	\N	69
Miss Whitney Towne	553384657348	\N	70
Ms. Cassandra Herman	558595294685	\N	71
Melinda Schaden Sr.	556362783138	\N	72
Dora Borer	556891157053	\N	73
Miss Thomas Bechtelar	551195187195	\N	74
Mrs. Howard Hagenes	550181030315	\N	75
Adrienne Welch II	557374133524	\N	76
Terrell Ebert	556311625323	\N	77
Sidney Runte	557676987770	\N	78
Timothy Zulauf	552271157208	\N	79
Jill Crooks III	553504653067	\N	80
Lamar Quigley	557857133090	\N	81
Diana Smith	559928037241	\N	82
Erika Reynolds	557647248920	\N	83
Audrey Stamm	557440931645	\N	84
Noah Jacobs DVM	550167005755	\N	85
Agnes Toy	553035460087	\N	86
Kristen Heidenreich	556722256849	\N	87
Ella Swaniawski	554750158214	\N	88
Lucy Denesik	556017556895	\N	89
Edwin Roob	556277306331	\N	90
Tami Moen	551284071144	\N	91
Tammy Farrell	550615181603	\N	92
Leroy Jacobs	553717155204	\N	93
Carol Dare	551203086285	\N	94
Otis Funk	556488668514	\N	95
Anne Bashirian	556182669048	\N	96
Melba Purdy	554382366441	\N	97
Jake Kris	550396539463	\N	98
Michael Lang	557435031570	\N	99
May Kuphal	557252069898	\N	100
Lorraine Greenfelder	554553245134	\N	101
Mr. Nichole Lehner	557827086577	\N	102
Edward Rowe	559023500298	\N	103
Faith Shanahan	558005469585	\N	104
Orville Stiedemann	558761063882	\N	105
Shannon Lang	558561833692	\N	106
Jeanette Schulist	559363351165	\N	107
Heather Dach	550276017872	\N	108
Wayne Reynolds	553542628940	\N	109
Lynn Lemke	555581984473	\N	110
Jeffrey Green	554099871303	\N	111
Felipe Swift	557146429851	\N	112
Guy Schuster	551225314098	\N	113
Minnie Cremin	552254224822	\N	114
Jared Yost	556284618365	\N	115
Luther King DVM	554837647089	\N	116
Rhonda Hickle	552359150939	\N	117
Jason Marks	552123478918	\N	118
Wilbert Kovacek	551274006533	\N	119
Toby Schmidt	552051294423	\N	120
Herman Ullrich	554909479788	\N	121
David Streich	553277743980	\N	122
Lauren Jakubowski	550221278402	\N	123
Mrs. Elsie Pfannerstill	556726388439	\N	124
Gordon Jast	556068393769	\N	125
Wilson Hegmann	556128596718	\N	126
Loretta Hintz	556975309120	\N	127
Freda Rice	551490676784	\N	128
Edward Brakus	556607446902	\N	129
Miss Margaret Spinka	556027660610	\N	130
Joy Bergnaum Sr.	551839268279	\N	131
Monique MacGyver	556442852447	\N	132
Elbert Pfannerstill	558393297000	\N	133
Armando Wiza	558548470444	\N	134
Dwayne Nienow	558384798122	\N	135
Ms. Colin Grady	554862381400	\N	136
Clinton Willms	555446285664	\N	137
Monique Stroman	554378829880	\N	138
Mr. Walter Bartoletti	556767179436	\N	139
Timothy Maggio	555853871502	\N	140
Ms. Alexandra McCullough	554084363337	\N	141
Kim Beer	554478804078	\N	142
Robert Wilkinson	556499571689	\N	143
Mattie Yundt	551768296700	\N	144
Miss Marcus Sipes	553166781100	\N	145
William Cronin	553808141465	\N	146
Doyle Emard	552234085165	\N	147
George Schamberger	559185507264	\N	148
Jana Ortiz Sr.	559876604934	\N	149
Melissa Sporer	551328486076	\N	150
Kelli Zboncak	553809919254	\N	151
Bonnie Koelpin	556829894748	\N	152
Joanna Gutkowski	558248238596	\N	153
Rebecca Weber	552522536239	\N	154
Ramon Maggio	556191686708	\N	155
Rita Crona DDS	550037017016	\N	156
Merle Padberg	557467108383	\N	157
Dr. Leonard Macejkovic	555984473860	\N	158
Jerald Will Sr.	552462698171	\N	159
Becky McCullough	557861820574	\N	160
Kristi Herman	557033261513	\N	161
Katie Heathcote III	556471703063	\N	162
Ella Schmeler	551129650788	\N	163
Bethany Blick	554031513809	\N	164
Manuel Botsford	550830452257	\N	165
Mr. Stuart Tromp	550146640383	\N	166
Casey Lesch	554251033471	\N	167
Scott Rice	559287194706	\N	168
Lillie Hagenes IV	558652115379	\N	169
Roy Cassin	558735417695	\N	170
Phyllis Daugherty	552568681913	\N	171
Hugo Parker	559991830225	\N	172
Donnie Halvorson	550104125419	\N	173
Alonzo Walker	552533083070	\N	174
Ivan Welch	552623064759	\N	175
Janie Leannon	552857665461	\N	176
Jean Cronin IV	557483286781	\N	177
Beatrice Kulas	550632697829	\N	178
Keith Langworth	552188834013	\N	179
Virginia Predovic	558109512098	\N	180
Glenn Fritsch	552065044352	\N	181
Ms. Francis Gleichner	552523865418	\N	182
Dr. Sheila Runte	553915569787	\N	183
Mrs. Lee Russel	555333840386	\N	184
Susie Kling	551368633798	\N	185
Raquel Pagac	554757315736	\N	186
Meredith Ledner	555024260053	\N	187
Ms. Constance Gutkowski	557731476605	\N	188
Dr. Elsie Bartell	558948765248	\N	189
Mr. Jordan Padberg	556663216953	\N	190
Ms. Vanessa Kunde	554706398324	\N	191
Melba Schumm	550429218120	\N	192
Bernice Rohan	555313760243	\N	193
Mr. Wilma Johnson	550991793592	\N	194
Stephanie Borer III	554070823407	\N	195
Nora Bechtelar	554647218590	\N	196
Minnie Lowe	555915481509	\N	197
Whitney Nikolaus	554357135472	\N	198
Rafael Ratke	550112152016	\N	199
Jean Ritchie	550002170479	\N	200
Jeff Morar	557511967230	\N	201
Andy Ankunding	558575224784	\N	202
Edward Dickinson	551871887849	\N	203
Craig Cummings	553045378199	\N	204
Patricia Gerlach	552765166046	\N	205
Margarita Feest	554172141828	\N	206
Guy Rice	557751073766	\N	207
Dr. Julia Spinka	550627092144	\N	208
Enrique Smith	554278423495	\N	209
Sherry Larkin	556673217160	\N	210
Marc Haley	554620865517	\N	211
Lauren Schuster	554506083934	\N	212
Joanne Paucek	552317915167	\N	213
Gwen Wolff	554926299319	\N	214
Andres de la Vega	2284567868	\N	215
Jorge Vergara	2284567490	\N	216
Ximena Lopez	2254673423	\N	217
Jose Antonio Ramirez	2290987895	\N	218
Gissel Ojeda	22810432902	\N	219
Antonio Aguilar	2291483928	\N	220
Cliente	22398042389	\N	221
\.


--
-- Name: appointment_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.appointment_id_seq', 73, true);


--
-- Name: availabilitytimeslot_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.availabilitytimeslot_id_seq', 76, true);


--
-- Name: notificationbase_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.notificationbase_id_seq', 1, false);


--
-- Name: scheduledservice_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.scheduledservice_id_seq', 100, true);


--
-- Name: service_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.service_id_seq', 22, true);


--
-- Name: serviceoffer_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.serviceoffer_id_seq', 270, true);


--
-- Name: useraccount_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.useraccount_id_seq', 221, true);


--
-- Name: Appointment PK_Appointment; Type: CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."Appointment"
    ADD CONSTRAINT "PK_Appointment" PRIMARY KEY (id);


--
-- Name: Assistant PK_Assistant; Type: CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."Assistant"
    ADD CONSTRAINT "PK_Assistant" PRIMARY KEY (id_user_account);


--
-- Name: ServiceOffer PK_AssistantService; Type: CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."ServiceOffer"
    ADD CONSTRAINT "PK_AssistantService" PRIMARY KEY (id);


--
-- Name: AvailabilityTimeSlot PK_AvailabilityTimeSlot; Type: CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."AvailabilityTimeSlot"
    ADD CONSTRAINT "PK_AvailabilityTimeSlot" PRIMARY KEY (id);


--
-- Name: Client PK_Client; Type: CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."Client"
    ADD CONSTRAINT "PK_Client" PRIMARY KEY (id_user_account);


--
-- Name: NotificationBase PK_Notification; Type: CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."NotificationBase"
    ADD CONSTRAINT "PK_Notification" PRIMARY KEY (id);


--
-- Name: ScheduledService PK_ScheduledService; Type: CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."ScheduledService"
    ADD CONSTRAINT "PK_ScheduledService" PRIMARY KEY (id);


--
-- Name: Service PK_Service; Type: CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."Service"
    ADD CONSTRAINT "PK_Service" PRIMARY KEY (id);


--
-- Name: UserAccount PK_UserAccount; Type: CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."UserAccount"
    ADD CONSTRAINT "PK_UserAccount" PRIMARY KEY (id);


--
-- Name: IXFK_AppointmentAssistantService_AssistantService; Type: INDEX; Schema: public; Owner: appdbuser
--

CREATE INDEX "IXFK_AppointmentAssistantService_AssistantService" ON public."ScheduledService" USING btree ("id_serviceOffer");


--
-- Name: IXFK_AppointmentNotification_Notification; Type: INDEX; Schema: public; Owner: appdbuser
--

CREATE INDEX "IXFK_AppointmentNotification_Notification" ON public."AppointmentNotification" USING btree (id_notification);


--
-- Name: IXFK_AppointmentService_Appointment; Type: INDEX; Schema: public; Owner: appdbuser
--

CREATE INDEX "IXFK_AppointmentService_Appointment" ON public."ScheduledService" USING btree (id_appointment);


--
-- Name: IXFK_Appointment_Client; Type: INDEX; Schema: public; Owner: appdbuser
--

CREATE INDEX "IXFK_Appointment_Client" ON public."Appointment" USING btree (id_client);


--
-- Name: IXFK_AssistantService_Assistant; Type: INDEX; Schema: public; Owner: appdbuser
--

CREATE INDEX "IXFK_AssistantService_Assistant" ON public."ServiceOffer" USING btree (id_assistant);


--
-- Name: IXFK_AssistantService_Service; Type: INDEX; Schema: public; Owner: appdbuser
--

CREATE INDEX "IXFK_AssistantService_Service" ON public."ServiceOffer" USING btree (id_service);


--
-- Name: IXFK_Assistant_UserAccount; Type: INDEX; Schema: public; Owner: appdbuser
--

CREATE INDEX "IXFK_Assistant_UserAccount" ON public."Assistant" USING btree (id_user_account);


--
-- Name: IXFK_AvailabilityTimeSlot_Assistant; Type: INDEX; Schema: public; Owner: appdbuser
--

CREATE INDEX "IXFK_AvailabilityTimeSlot_Assistant" ON public."AvailabilityTimeSlot" USING btree (id_assistant);


--
-- Name: IXFK_Client_UserAccount; Type: INDEX; Schema: public; Owner: appdbuser
--

CREATE INDEX "IXFK_Client_UserAccount" ON public."Client" USING btree (id_user_account);


--
-- Name: IXFK_GeneralNotification_NotificationBase; Type: INDEX; Schema: public; Owner: appdbuser
--

CREATE INDEX "IXFK_GeneralNotification_NotificationBase" ON public."GeneralNotification" USING btree (id_notification);


--
-- Name: IXFK_NotificationBaseUserAccount_NotificationBase; Type: INDEX; Schema: public; Owner: appdbuser
--

CREATE INDEX "IXFK_NotificationBaseUserAccount_NotificationBase" ON public."NotificationRecipient" USING btree (id_notification);


--
-- Name: IXFK_NotificationBaseUserAccount_UserAccount; Type: INDEX; Schema: public; Owner: appdbuser
--

CREATE INDEX "IXFK_NotificationBaseUserAccount_UserAccount" ON public."NotificationRecipient" USING btree (id_user_account);


--
-- Name: IXFK_Notification_Appointment; Type: INDEX; Schema: public; Owner: appdbuser
--

CREATE INDEX "IXFK_Notification_Appointment" ON public."AppointmentNotification" USING btree (id_appointment);


--
-- Name: IXFK_SystemNotification_NotificationBase; Type: INDEX; Schema: public; Owner: appdbuser
--

CREATE INDEX "IXFK_SystemNotification_NotificationBase" ON public."SystemNotification" USING btree (id_notification);


--
-- Name: IXFK_UnavailableTimeSlot_AvailabilityTimeSlot; Type: INDEX; Schema: public; Owner: appdbuser
--

CREATE INDEX "IXFK_UnavailableTimeSlot_AvailabilityTimeSlot" ON public."UnavailableTimeSlot" USING btree (id_availability_time_slot);


--
-- Name: IXFK_UserInformation_UserAccount; Type: INDEX; Schema: public; Owner: appdbuser
--

CREATE INDEX "IXFK_UserInformation_UserAccount" ON public."UserInformation" USING btree (id_user);


--
-- Name: ScheduledService FK_AppointmentAssistantService_AssistantService; Type: FK CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."ScheduledService"
    ADD CONSTRAINT "FK_AppointmentAssistantService_AssistantService" FOREIGN KEY ("id_serviceOffer") REFERENCES public."ServiceOffer"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: AppointmentNotification FK_AppointmentNotification_Notification; Type: FK CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."AppointmentNotification"
    ADD CONSTRAINT "FK_AppointmentNotification_Notification" FOREIGN KEY (id_notification) REFERENCES public."NotificationBase"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: ScheduledService FK_AppointmentService_Appointment; Type: FK CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."ScheduledService"
    ADD CONSTRAINT "FK_AppointmentService_Appointment" FOREIGN KEY (id_appointment) REFERENCES public."Appointment"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: Appointment FK_Appointment_Client; Type: FK CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."Appointment"
    ADD CONSTRAINT "FK_Appointment_Client" FOREIGN KEY (id_client) REFERENCES public."Client"(id_user_account) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: ServiceOffer FK_AssistantService_Assistant; Type: FK CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."ServiceOffer"
    ADD CONSTRAINT "FK_AssistantService_Assistant" FOREIGN KEY (id_assistant) REFERENCES public."Assistant"(id_user_account) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: ServiceOffer FK_AssistantService_Service; Type: FK CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."ServiceOffer"
    ADD CONSTRAINT "FK_AssistantService_Service" FOREIGN KEY (id_service) REFERENCES public."Service"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: Assistant FK_Assistant_UserAccount; Type: FK CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."Assistant"
    ADD CONSTRAINT "FK_Assistant_UserAccount" FOREIGN KEY (id_user_account) REFERENCES public."UserAccount"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: AvailabilityTimeSlot FK_AvailabilityTimeSlot_Assistant; Type: FK CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."AvailabilityTimeSlot"
    ADD CONSTRAINT "FK_AvailabilityTimeSlot_Assistant" FOREIGN KEY (id_assistant) REFERENCES public."Assistant"(id_user_account) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: Client FK_Client_UserAccount; Type: FK CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."Client"
    ADD CONSTRAINT "FK_Client_UserAccount" FOREIGN KEY (id_user_account) REFERENCES public."UserAccount"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: GeneralNotification FK_GeneralNotification_NotificationBase; Type: FK CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."GeneralNotification"
    ADD CONSTRAINT "FK_GeneralNotification_NotificationBase" FOREIGN KEY (id_notification) REFERENCES public."NotificationBase"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: NotificationRecipient FK_NotificationBaseUserAccount_NotificationBase; Type: FK CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."NotificationRecipient"
    ADD CONSTRAINT "FK_NotificationBaseUserAccount_NotificationBase" FOREIGN KEY (id_notification) REFERENCES public."NotificationBase"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: NotificationRecipient FK_NotificationBaseUserAccount_UserAccount; Type: FK CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."NotificationRecipient"
    ADD CONSTRAINT "FK_NotificationBaseUserAccount_UserAccount" FOREIGN KEY (id_user_account) REFERENCES public."UserAccount"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: AppointmentNotification FK_Notification_Appointment; Type: FK CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."AppointmentNotification"
    ADD CONSTRAINT "FK_Notification_Appointment" FOREIGN KEY (id_appointment) REFERENCES public."Appointment"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: SystemNotification FK_SystemNotification_NotificationBase; Type: FK CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."SystemNotification"
    ADD CONSTRAINT "FK_SystemNotification_NotificationBase" FOREIGN KEY (id_notification) REFERENCES public."NotificationBase"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: UnavailableTimeSlot FK_UnavailableTimeSlot_AvailabilityTimeSlot; Type: FK CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."UnavailableTimeSlot"
    ADD CONSTRAINT "FK_UnavailableTimeSlot_AvailabilityTimeSlot" FOREIGN KEY (id_availability_time_slot) REFERENCES public."AvailabilityTimeSlot"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: UserInformation FK_UserInformation_UserAccount; Type: FK CONSTRAINT; Schema: public; Owner: appdbuser
--

ALTER TABLE ONLY public."UserInformation"
    ADD CONSTRAINT "FK_UserInformation_UserAccount" FOREIGN KEY (id_user) REFERENCES public."UserAccount"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

