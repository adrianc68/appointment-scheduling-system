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
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    id_client integer,
    start_date timestamp without time zone,
    end_date timestamp without time zone
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
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    id_assistant integer NOT NULL,
    status public."AvailabilityTimeSlotStatusType",
    start_date timestamp without time zone,
    end_date timestamp without time zone
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
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
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
    changed_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public."NotificationRecipient" OWNER TO appdbuser;

--
-- Name: ScheduledService; Type: TABLE; Schema: public; Owner: appdbuser
--

CREATE TABLE public."ScheduledService" (
    "id_serviceOffer" integer,
    id_appointment integer,
    start_date timestamp without time zone,
    end_date timestamp without time zone,
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
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
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
    start_date timestamp without time zone,
    end_date timestamp without time zone,
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
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
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
\.


--
-- Data for Name: AvailabilityTimeSlot; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."AvailabilityTimeSlot" (id, uuid, created_at, id_assistant, status, start_date, end_date) FROM stdin;
\.


--
-- Data for Name: Client; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."Client" (id_user_account, status) FROM stdin;
1	\N
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
\.


--
-- Data for Name: Service; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."Service" (description, minutes, name, price, id, uuid, created_at, status) FROM stdin;
\.


--
-- Data for Name: ServiceOffer; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."ServiceOffer" (id_assistant, id_service, uuid, id, status) FROM stdin;
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
\.


--
-- Data for Name: UserAccount; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."UserAccount" (email, password, username, created_at, id, role, uuid, status) FROM stdin;
angeladriancamalgarcia@hotmail.com	$2a$11$H87g0VAEVL5fPSmfnwuvWOkF2TPOW2LocunGyZnkT/jgfj8YshBGq	adrian	2025-08-22 01:23:24.971133	1	ADMINISTRATOR	0198cf5f-9465-7fd9-bc05-8288ba9371f7	ENABLED
\.


--
-- Data for Name: UserInformation; Type: TABLE DATA; Schema: public; Owner: appdbuser
--

COPY public."UserInformation" (name, phone_number, filepath, id_user) FROM stdin;
Administrator	2281046161	\N	1
\.


--
-- Name: appointment_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.appointment_id_seq', 1, false);


--
-- Name: availabilitytimeslot_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.availabilitytimeslot_id_seq', 1, false);


--
-- Name: notificationbase_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.notificationbase_id_seq', 1, false);


--
-- Name: scheduledservice_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.scheduledservice_id_seq', 1, false);


--
-- Name: service_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.service_id_seq', 1, false);


--
-- Name: serviceoffer_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.serviceoffer_id_seq', 1, false);


--
-- Name: useraccount_id_seq; Type: SEQUENCE SET; Schema: public; Owner: appdbuser
--

SELECT pg_catalog.setval('public.useraccount_id_seq', 1, true);


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

