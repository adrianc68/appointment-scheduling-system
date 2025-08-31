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
CONFIRMED	44.13	0198e849-273f-77a0-af0f-afebbeac74c7	32	2025-08-26 21:29:25.44322+00	2	2025-08-25 15:30:00+00	2025-08-25 15:40:00+00
CONFIRMED	44.13	0198e8a4-bad4-7b90-a75f-31ebc285b1d1	33	2025-08-26 23:09:26.753759+00	2	2025-08-25 23:07:54+00	2025-08-25 23:17:54+00
CONFIRMED	88.26	0198e8a9-c6e9-7f5a-afcf-f9d504a5eb8d	34	2025-08-26 23:14:57.385465+00	2	2025-08-26 01:00:00+00	2025-08-26 01:20:00+00
CONFIRMED	756.45	0198f849-1b9e-79e6-840c-45a9f3e361bb	35	2025-08-30 00:03:17.710185+00	2	2025-08-28 01:03:13+00	2025-08-28 01:13:13+00
CONFIRMED	1726.03	0198f84c-a6f0-7835-811e-d7412f714f89	36	2025-08-30 00:07:09.819002+00	3	2025-08-29 16:00:00+00	2025-08-29 16:30:00+00
CONFIRMED	756.45	0198f872-75fa-7bc3-baa9-a078f71442dd	37	2025-08-30 00:48:27.643128+00	3	2025-08-29 21:00:00+00	2025-08-29 21:10:00+00
CONFIRMED	229.72	0198f892-a2d0-7c1a-bfed-4c28b1c0be09	38	2025-08-30 01:23:36.271874+00	10	2025-08-29 16:30:00+00	2025-08-29 16:40:00+00
CONFIRMED	756.45	0198fbd0-a6a7-7e0e-b660-7f76ea85f46d	39	2025-08-30 16:30:12.140321+00	11	2025-08-27 16:30:05+00	2025-08-27 16:40:05+00
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
4	\N
5	\N
6	\N
7	\N
8	\N
\.


--
-- Data for Name: AvailabilityTimeSlot; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."AvailabilityTimeSlot" (id, uuid, created_at, id_assistant, status, start_date, end_date) FROM stdin;
41	0198ee26-ec88-74ec-9b67-fc70e9d03da4	2025-08-28 00:49:45.105667+00	5	ENABLED	2025-08-27 15:00:00+00	2025-08-28 03:00:00+00
42	0198f84b-9ce1-73e8-9d00-3d8bf5f14e83	2025-08-30 00:06:01.71795+00	8	DISABLED	2025-08-29 15:00:00+00	2025-08-30 03:00:00+00
\.


--
-- Data for Name: Client; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."Client" (id_user_account, status) FROM stdin;
1	\N
2	\N
3	\N
9	\N
10	\N
11	\N
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
163	32	2025-08-25 15:30:00+00	2025-08-25 15:40:00+00	Dynamic Implementation Director	44.13	10	0198e849-273f-7991-9039-65e9949e4fc7	41
163	33	2025-08-25 23:07:54+00	2025-08-25 23:17:54+00	Dynamic Implementation Director	44.13	10	0198e8a4-bad4-7be2-889d-59281530aaa1	42
163	34	2025-08-26 01:00:00+00	2025-08-26 01:10:00+00	Dynamic Implementation Director	44.13	10	0198e8a9-c6e9-7424-9f9f-2b2c8789e6f1	43
165	34	2025-08-26 01:10:00+00	2025-08-26 01:20:00+00	Dynamic Implementation Director	44.13	10	0198e8a9-c6e9-7230-a938-30fc5911279f	44
172	35	2025-08-28 01:03:13+00	2025-08-28 01:13:13+00	Corporate Web Planner	756.45	10	0198f849-1b9e-7d1f-b6cc-0dc97208e585	45
186	36	2025-08-29 16:00:00+00	2025-08-29 16:10:00+00	Corporate Web Planner	756.45	10	0198f84c-a6f0-7ed9-b6a5-0670963bcc2d	46
184	36	2025-08-29 16:10:00+00	2025-08-29 16:20:00+00	Dynamic Implementation Director	44.13	10	0198f84c-a6f0-757e-b922-e1c2b5994b7b	47
187	36	2025-08-29 16:20:00+00	2025-08-29 16:30:00+00	Forward Directives Facilitator	925.45	10	0198f84c-a6f0-7028-8f3a-03523d70da8b	48
186	37	2025-08-29 21:00:00+00	2025-08-29 21:10:00+00	Corporate Web Planner	756.45	10	0198f872-75fa-7123-8884-32cdaff1dd6c	49
188	38	2025-08-29 16:30:00+00	2025-08-29 16:40:00+00	Human Markets Analyst	229.72	10	0198f892-a2d0-7aa8-9fd0-da1123c6cb70	50
172	39	2025-08-27 16:30:05+00	2025-08-27 16:40:05+00	Corporate Web Planner	756.45	10	0198fbd0-a6a7-78f4-a0d3-b99fa6ba1eda	51
\.


--
-- Data for Name: Service; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."Service" (description, minutes, name, price, id, uuid, created_at, status) FROM stdin;
matrix channels	10	Dynamic Implementation Director	44.13	1	0198d45b-0acb-7652-b3f8-01359af70bb8	2025-08-23 00:36:33.156784+00	ENABLED
programming Practical	10	Lead Intranet Officer	202.33	2	0198d45b-1665-737c-b99f-2b2064ee3e25	2025-08-23 00:36:36.074489+00	ENABLED
Fresh Principal adapter SMS green	10	Corporate Web Planner	756.45	3	0198d45b-1bf7-7e79-b63c-31e2d795183a	2025-08-23 00:36:37.506872+00	ENABLED
Investment synthesizing Hills Shilling	10	Human Markets Analyst	229.72	4	0198d45b-204f-7fba-8b07-523ba484f235	2025-08-23 00:36:38.619525+00	ENABLED
magenta Metal Director Rubber	10	Forward Directives Facilitator	925.45	5	0198d45b-250d-7af0-9065-7784cef823b4	2025-08-23 00:36:39.816869+00	ENABLED
synergistic calculating Berkshire Shirt	10	National Group Manager	425.13	6	0198d45b-2e22-754a-839f-267ad5c1c82a	2025-08-23 00:36:42.147414+00	ENABLED
Engineer bifurcated	10	Legacy Web Executive	758.56	7	0198d45b-3347-7d1a-956e-e2643c76a576	2025-08-23 00:36:43.465982+00	ENABLED
web Movies Niger Baby Designer	10	Dynamic Tactics Developer	95.16	8	0198d45b-389d-7b9e-a252-d768393c427b	2025-08-23 00:36:44.836754+00	ENABLED
District Industrial withdrawal	10	Investor Operations Planner	380.03	9	0198d45b-3da0-742e-b7cf-54f6c1ab8994	2025-08-23 00:36:46.119457+00	ENABLED
Faroe protocol virtual Sleek Web	10	Global Functionality Designer	26.1	10	0198d45b-4251-74e7-8a9d-f9e3bb6a8a6a	2025-08-23 00:36:47.3189+00	ENABLED
Bike Nakfa SCSI Iraq	110	Legacy Tactics Facilitator	504.76	11	0198dfff-48f8-71b8-86a1-0638cb89fcc9	2025-08-25 06:51:46.321538+00	ENABLED
\.


--
-- Data for Name: ServiceOffer; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."ServiceOffer" (id_assistant, id_service, uuid, id, status) FROM stdin;
6	1	0198e8c5-ec72-77f5-b7a7-b99d8ed8b4aa	167	ENABLED
8	1	0198e904-ab10-71a9-8cb7-9c33b56890c4	168	DELETED
5	2	0198e991-6ad6-78c9-8c65-66288eefb8c1	171	ENABLED
5	3	0198e991-7f91-7b61-adbe-c77eaf13fd39	172	ENABLED
5	4	0198e991-83f2-7c2b-b942-1fd5a5c30781	173	ENABLED
6	2	0198e991-b436-76af-8bdb-6678de2036a3	174	ENABLED
5	6	0198ea87-52d6-7d56-b378-a56893bb1e73	176	DELETED
5	5	0198ea87-4f05-72ea-8c76-e1492c898ddb	175	DELETED
5	7	0198ea87-5b44-7a83-a82e-da78b4b4012c	177	ENABLED
5	1	0198e991-6212-7e0e-aebd-17da58039f36	170	ENABLED
6	3	0198eac7-1c17-79db-a411-1b619255dbc4	178	DELETED
6	4	0198eac7-2287-74e8-b3e0-ea7f259b35cc	179	ENABLED
5	5	0198ead4-d72f-77ee-9e41-4cb7a2c38abe	180	DELETED
5	6	0198ead4-ebda-7d7f-8463-acce332d8911	181	ENABLED
5	8	0198ead4-f0a7-727e-8cea-2b699da6b7e7	182	ENABLED
5	9	0198ead4-f5d2-74a5-9dfe-cb2a39c66f57	183	ENABLED
8	1	0198ee4d-ab78-7c3d-b712-4f5036842507	184	ENABLED
8	2	0198ee4d-af15-7404-b73f-f68f31a76e38	185	ENABLED
8	3	0198ee4d-b152-7fa7-bd18-b680b9cbc337	186	ENABLED
8	5	0198ee4d-b340-702f-9fcb-c4b0682c141e	187	ENABLED
8	4	0198ee4d-b636-7838-8e5c-70060a8c7fa5	188	ENABLED
5	5	0198f2cb-6445-7d1d-acf5-0530921b2396	189	DELETED
7	1	0198e93f-e5df-7236-a84d-28880dc491aa	169	ENABLED
7	2	0198f300-5ab4-7c5d-a3ca-4d5d4ccf49f8	190	ENABLED
7	4	0198f300-615e-7693-a0f5-a05b1747cefa	191	ENABLED
7	5	0198f300-63ae-7d28-9c52-26a7376147fc	192	ENABLED
7	6	0198f300-650b-738d-8eff-a20b488581c5	193	ENABLED
7	7	0198f300-66b9-7d2c-8fc8-df2fcdaa40e6	194	ENABLED
7	8	0198f300-6725-75d0-8369-c8365ecaa714	195	ENABLED
7	9	0198f300-67c3-715c-8f2e-9a4480aefe59	196	ENABLED
7	10	0198f300-6856-70ec-adba-dc8d122cd233	197	ENABLED
7	11	0198f300-6d5d-724e-b6b6-c5a0c2975d87	199	DELETED
7	3	0198f300-6b16-705d-a012-d2fa08dc1c61	198	DELETED
7	11	0198f300-8813-7519-a55d-69e9b6c3aff8	200	ENABLED
6	10	0198f300-ad59-7baa-9675-84f651ee9f8a	201	ENABLED
6	9	0198f300-af8c-773b-b650-9453f70639eb	202	ENABLED
6	8	0198f300-b162-7046-af3a-0588ef921ead	203	ENABLED
6	7	0198f300-b228-76e2-a692-050439ad5d23	204	ENABLED
6	6	0198f300-b31b-7aa9-81b2-27fceae51202	205	ENABLED
6	5	0198f300-b4c2-7c66-a08f-7cc323b39059	206	ENABLED
6	3	0198f300-b5de-7b9c-9142-ba01f239259b	207	ENABLED
6	11	0198f300-b9ae-7a19-8f58-1c5609307775	208	DELETED
5	3	0198e8bf-8848-75d5-b290-f67b8a39934d	166	DELETED
5	2	0198e8a7-e19c-7876-8090-c83068177d37	164	DELETED
5	1	0198e82d-5acc-7d9e-9cb1-ba181bf61545	163	DELETED
6	1	0198e8a7-ed66-7ab6-a133-bf053cae5c55	165	DELETED
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
2025-08-28 20:00:00+00	2025-08-28 21:00:00+00	41
\.


--
-- Data for Name: UserAccount; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."UserAccount" (email, password, username, created_at, id, role, uuid, status) FROM stdin;
Tyrel_Botsford@gmail.com	$2a$11$pAsTUGFOcIikojANHlNleudy/uuM8.Pd/zyAsgmr0t8ymXftkl.FW	Amina_Halvorson15	2025-08-23 00:34:41.623085+00	2	CLIENT	0198d459-5664-7cbf-8b8d-acb2e3c5fabf	ENABLED
Regan92@yahoo.com	$2a$11$SwzevYh9AsvUZqzwgCeLcuAH.hQKj3s5d6m95Ip.u3v/L3U9QNWOG	Mavis0	2025-08-23 00:34:44.413449+00	3	CLIENT	0198d459-614b-7de2-89c0-cfeadef950d7	ENABLED
adrian@hola.com	$2a$11$rbKmGlnbJI42FHOPLmz5Qe1RUFO6calh7PoWQzfOMzqkMIL.jbfly	adrian	2025-08-23 00:34:12.542223+00	1	ADMINISTRATOR	0198d458-e077-7856-a908-d73cb06bd670	ENABLED
Devon56@hotmail.com	$2a$11$ugd9ArDVUIWBpfNsqtJV9OcYbMLjf3pkk6GAmlz4vjHTlM4PhWqBG	Ubaldo_Hegmann	2025-08-25 05:46:35.323657+00	5	ASSISTANT	0198dfc3-9a1a-709a-b76a-c863152be0c3	ENABLED
Estevan_Dicki25@gmail.com	$2a$11$KzdYXRMT26xe3ByqB9gKLeZ2uSMQMIRaUMViMEgsZ13JPOU65Tm/S	Lucas_Will	2025-08-25 06:03:53.360208+00	6	ASSISTANT	0198dfd3-71de-7130-817a-8a4c851aea9e	ENABLED
Osborne.Donnelly@gmail.com	$2a$11$z5IY8A9pfZzzMuAbyzTDi./eQDonIvuATKfhZ3gKHImiXLSE1XgWi	Lance39	2025-08-25 06:06:16.506729+00	7	ASSISTANT	0198dfd5-a0fb-7833-9908-a1db982653d7	ENABLED
Dortha.Schumm39@hotmail.com	$2a$11$A/9yTXIE2vZOayEISqLcAerFRDNqlJU1x1rPObxWhtHUMDQpeDxtO	Asa.Keeling84	2025-08-23 00:36:21.439452+00	4	ASSISTANT	0198d45a-dc82-7894-868f-9cb9a89384a9	DISABLED
Alfredo_Parisian59@gmail.com	$2a$11$17MR0Kw.FahmjhdoHLlX0uu9N.g9mG6KRIwkTN5EkTHkSwplwMTL.	Belle_Bergstrom21	2025-08-25 23:30:19.815477+00	8	ASSISTANT	0198e391-7be1-7056-a83c-6e6e0ace8e45	ENABLED
Trevion.Torphy25@hotmail.com	$2a$11$5WBm44fbb8DUewgS9yq7Ruqs440ACMjC/JyTAEPi8ti0cm6rxQeie	Lottie14	2025-08-30 00:45:15.665424+00	9	CLIENT	0198f86f-874b-774b-b42b-12c0ddcc6574	ENABLED
Rachelle_Bergstrom67@gmail.com	$2a$11$JSDunk3dSeg8PDKM1Eee1e1x.YBoo8EARcdAfqGnIhoN9iDTbM5n.	Abby_Okuneva32	2025-08-30 00:45:17.021486+00	10	CLIENT	0198f86f-8caf-7ac6-b830-a85d1e4e00ac	ENABLED
Alessandra.Von@yahoo.com	$2a$11$HhhTIqPdbVD9bs5gUzncHu7SMn1k5.jzQZ5BbkmdeLYu9jsyByIp2	Durward47	2025-08-30 00:45:18.270758+00	11	CLIENT	0198f86f-9194-7b29-859b-87e11cfeb9b6	ENABLED
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
\.


--
-- Name: appointment_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.appointment_id_seq', 39, true);


--
-- Name: availabilitytimeslot_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.availabilitytimeslot_id_seq', 42, true);


--
-- Name: notificationbase_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.notificationbase_id_seq', 1, false);


--
-- Name: scheduledservice_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.scheduledservice_id_seq', 51, true);


--
-- Name: service_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.service_id_seq', 11, true);


--
-- Name: serviceoffer_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.serviceoffer_id_seq', 208, true);


--
-- Name: useraccount_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.useraccount_id_seq', 11, true);


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

