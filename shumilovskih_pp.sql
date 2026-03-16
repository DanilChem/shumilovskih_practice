--
-- PostgreSQL database dump
--

-- Dumped from database version 16.4
-- Dumped by pg_dump version 16.4
create database shumilovskih_pp;

\c shumilovskih_pp

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: app; Type: SCHEMA; Schema: -; Owner: app
--

CREATE SCHEMA app;


ALTER SCHEMA app OWNER TO app;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: partner_types; Type: TABLE; Schema: app; Owner: app
--

CREATE TABLE app.partner_types (
    partner_type_id integer NOT NULL,
    type_name character varying(100) NOT NULL
);


ALTER TABLE app.partner_types OWNER TO app;

--
-- Name: partner_types_partner_type_id_seq; Type: SEQUENCE; Schema: app; Owner: app
--

CREATE SEQUENCE app.partner_types_partner_type_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE app.partner_types_partner_type_id_seq OWNER TO app;

--
-- Name: partner_types_partner_type_id_seq; Type: SEQUENCE OWNED BY; Schema: app; Owner: app
--

ALTER SEQUENCE app.partner_types_partner_type_id_seq OWNED BY app.partner_types.partner_type_id;


--
-- Name: partners; Type: TABLE; Schema: app; Owner: app
--

CREATE TABLE app.partners (
    partner_id integer NOT NULL,
    company_name character varying(255) NOT NULL,
    partner_type_id integer NOT NULL,
    inn character varying(20),
    logo character varying(500),
    rating integer DEFAULT 0 NOT NULL,
    address character varying(500),
    director_name character varying(255),
    phone character varying(50),
    email character varying(255),
    sales_locations text,
    created_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT partners_rating_check CHECK ((rating >= 0))
);


ALTER TABLE app.partners OWNER TO app;

--
-- Name: partners_partner_id_seq; Type: SEQUENCE; Schema: app; Owner: app
--

CREATE SEQUENCE app.partners_partner_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE app.partners_partner_id_seq OWNER TO app;

--
-- Name: partners_partner_id_seq; Type: SEQUENCE OWNED BY; Schema: app; Owner: app
--

ALTER SEQUENCE app.partners_partner_id_seq OWNED BY app.partners.partner_id;


--
-- Name: products; Type: TABLE; Schema: app; Owner: app
--

CREATE TABLE app.products (
    product_id integer NOT NULL,
    product_name character varying(255) NOT NULL,
    article character varying(100),
    type character varying(100),
    description text,
    min_price numeric(10,2) NOT NULL,
    package_size character varying(100),
    weight numeric(10,2),
    standard_number character varying(100),
    created_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT products_min_price_check CHECK ((min_price >= (0)::numeric)),
    CONSTRAINT products_weight_check CHECK ((weight >= (0)::numeric))
);


ALTER TABLE app.products OWNER TO app;

--
-- Name: products_product_id_seq; Type: SEQUENCE; Schema: app; Owner: app
--

CREATE SEQUENCE app.products_product_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE app.products_product_id_seq OWNER TO app;

--
-- Name: products_product_id_seq; Type: SEQUENCE OWNED BY; Schema: app; Owner: app
--

ALTER SEQUENCE app.products_product_id_seq OWNED BY app.products.product_id;


--
-- Name: sales_history; Type: TABLE; Schema: app; Owner: app
--

CREATE TABLE app.sales_history (
    sale_id integer NOT NULL,
    partner_id integer NOT NULL,
    product_id integer NOT NULL,
    quantity integer NOT NULL,
    sale_date date DEFAULT CURRENT_DATE NOT NULL,
    total_amount numeric(12,2) NOT NULL,
    created_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT sales_history_quantity_check CHECK ((quantity > 0)),
    CONSTRAINT sales_history_total_amount_check CHECK ((total_amount >= (0)::numeric))
);


ALTER TABLE app.sales_history OWNER TO app;

--
-- Name: sales_history_sale_id_seq; Type: SEQUENCE; Schema: app; Owner: app
--

CREATE SEQUENCE app.sales_history_sale_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE app.sales_history_sale_id_seq OWNER TO app;

--
-- Name: sales_history_sale_id_seq; Type: SEQUENCE OWNED BY; Schema: app; Owner: app
--

ALTER SEQUENCE app.sales_history_sale_id_seq OWNED BY app.sales_history.sale_id;


--
-- Name: partner_types partner_type_id; Type: DEFAULT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.partner_types ALTER COLUMN partner_type_id SET DEFAULT nextval('app.partner_types_partner_type_id_seq'::regclass);


--
-- Name: partners partner_id; Type: DEFAULT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.partners ALTER COLUMN partner_id SET DEFAULT nextval('app.partners_partner_id_seq'::regclass);


--
-- Name: products product_id; Type: DEFAULT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.products ALTER COLUMN product_id SET DEFAULT nextval('app.products_product_id_seq'::regclass);


--
-- Name: sales_history sale_id; Type: DEFAULT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.sales_history ALTER COLUMN sale_id SET DEFAULT nextval('app.sales_history_sale_id_seq'::regclass);


--
-- Data for Name: partner_types; Type: TABLE DATA; Schema: app; Owner: app
--

COPY app.partner_types (partner_type_id, type_name) FROM stdin;
1	Строительный магазин
2	Оптовый склад
3	Дистрибьютор
4	Интернет-магазин
5	Розничная сеть
6	4545
\.


--
-- Data for Name: partners; Type: TABLE DATA; Schema: app; Owner: app
--

COPY app.partners (partner_id, company_name, partner_type_id, inn, logo, rating, address, director_name, phone, email, sales_locations, created_date, modified_date) FROM stdin;
1	СтройДом	1	7701123456	/logos/stroydom.png	5	г. Москва, ул. Строителей, 10	Иванов Иван Иванович	+7 (495) 123-45-67	info@stroydom.ru	Москва, Московская область	2026-03-16 00:03:05.854922	2026-03-16 00:03:05.854922
2	ОптТорг	2	7701987654	/logos/opttorg.png	4	г. Санкт-Петербург, пр. Промышленный, 5	Петров Петр Петрович	+7 (812) 987-65-43	sales@opttorg.ru	СЗФО	2026-03-16 00:03:05.854922	2026-03-16 00:03:05.854922
3	Домовой	5	7702123450	/logos/domovoy.png	5	г. Екатеринбург, ул. Ленина, 20	Сидоров Сидор Сидорович	+7 (343) 222-33-44	info@domovoy.ru	Урал	2026-03-16 00:03:05.854922	2026-03-16 00:03:05.854922
5	СтройАрсенал	4	7704123452	/logos/stroyarsenal.png	4	г. Новосибирск, ул. Мира, 30	Васильев Василий	+7 (383) 444-55-66	shop@stroyarsenal.ru	Сибирь	2026-03-16 00:03:05.854922	2026-03-16 00:03:05.854922
6	Уютный Дом	1	7705123453	/logos/uytdom.png	3	г. Ростов-на-Дону, ул. Садовая, 12	Алексеева Анна	+7 (863) 777-88-99	info@uytdom.ru	ЮФО	2026-03-16 00:03:05.854922	2026-03-16 00:03:05.854922
7	Ремонт-Сервис	3	7706123454	/logos/remont.png	4	г. Нижний Новгород, ул. Промышленная, 7	Михайлов Михаил	+7 (831) 333-44-55	mih@remont.ru	ПФО	2026-03-16 00:03:05.854922	2026-03-16 00:03:05.854922
8	ТД Паркет	2	7707123455	/logos/tdparket.png	5	г. Воронеж, ул. 20 лет Октября, 50	Егоров Егор	+7 (473) 222-33-44	tdparket@mail.ru	Центр	2026-03-16 00:03:05.854922	2026-03-16 00:03:05.854922
9	Онлайн-Пол	4	7708123456	/logos/onlinepol.png	4	г. Краснодар, ул. Красная, 1	Сергеев Сергей	+7 (861) 555-66-77	info@online-pol.ru	Интернет	2026-03-16 00:03:05.854922	2026-03-16 00:03:05.854922
10	Доступный пол	1	7709123457	/logos/dostupnyi.png	3	г. Самара, ул. Дачная, 8	Федоров Федор	+7 (846) 444-55-66	fedorov@pol.ru	Самара и область	2026-03-16 00:03:05.854922	2026-03-16 00:03:05.854922
4	Пол-Мастер	3	7703123451	/logos/polmaster.png	5	г. Казань, ул. Машиностроителей, 15	Кузнецов Андрей	+7 (843) 555-66-77	zakaz@polmaster.ru	Татарстан, Поволжье	2026-03-16 00:03:05.854922	2026-03-16 00:10:09.483704
11	БрухныйПол	2	454554343	C:\\Users\\danil\\OneDrive\\Рабочий стол\\Poop\\0153326d7c9c10dc1e51d82a14868bca.jpg	4	ул. Уинская 68	Колотушкин Дмитрий Алексеевич	+7 (234) 495 34 34	ttr@gmail.com	Где-то	2026-03-16 00:14:36.00789	2026-03-16 00:16:29.573253
\.


--
-- Data for Name: products; Type: TABLE DATA; Schema: app; Owner: app
--

COPY app.products (product_id, product_name, article, type, description, min_price, package_size, weight, standard_number, created_date, modified_date) FROM stdin;
1	Ламинат Дуб белый	LAM-001	Ламинат	Ламинат 33 класс, толщина 8 мм	850.00	8 шт/упак	12.50	EN 13329	2026-03-16 00:02:58.338056	2026-03-16 00:02:58.338056
2	Ламинат Орех тёмный	LAM-002	Ламинат	Ламинат 33 класс, толщина 8 мм	890.00	8 шт/упак	12.50	EN 13329	2026-03-16 00:02:58.338056	2026-03-16 00:02:58.338056
3	Паркетная доска Ясень	PAR-001	Паркет	Паркетная доска трёхслойная, 14 мм	2100.00	4 шт/упак	20.00	EN 14342	2026-03-16 00:02:58.338056	2026-03-16 00:02:58.338056
4	Паркетная доска Дуб	PAR-002	Паркет	Паркетная доска трёхслойная, 14 мм	2300.00	4 шт/упак	20.00	EN 14342	2026-03-16 00:02:58.338056	2026-03-16 00:02:58.338056
5	Виниловая плитка Серая	VIN-001	Винил	ПВХ плитка замковая	650.00	10 шт/упак	15.00	EN 649	2026-03-16 00:02:58.338056	2026-03-16 00:02:58.338056
6	Виниловая плитка Бежевая	VIN-002	Винил	ПВХ плитка замковая	670.00	10 шт/упак	15.00	EN 649	2026-03-16 00:02:58.338056	2026-03-16 00:02:58.338056
7	Линолеум бытовой	LIN-001	Линолеум	Бытовой линолеум, ширина 2.5 м	320.00	пог. м	8.00	EN 548	2026-03-16 00:02:58.338056	2026-03-16 00:02:58.338056
8	Линолеум коммерческий	LIN-002	Линолеум	Коммерческий линолеум, ширина 2.5 м	450.00	пог. м	9.50	EN 548	2026-03-16 00:02:58.338056	2026-03-16 00:02:58.338056
9	Ковровая плитка	KOV-001	Ковролин	Ковровая плитка модульная	550.00	шт	2.30	EN 1307	2026-03-16 00:02:58.338056	2026-03-16 00:02:58.338056
10	Кварцвинил	KVZ-001	Кварцвинил	Кварцвиниловая плитка	980.00	шт	3.00	EN 14041	2026-03-16 00:02:58.338056	2026-03-16 00:02:58.338056
\.


--
-- Data for Name: sales_history; Type: TABLE DATA; Schema: app; Owner: app
--

COPY app.sales_history (sale_id, partner_id, product_id, quantity, sale_date, total_amount, created_date) FROM stdin;
1	1	1	10	2025-12-10	8500.00	2026-03-16 00:03:12.132025
2	1	2	5	2025-12-15	4450.00	2026-03-16 00:03:12.132025
3	1	5	8	2026-01-20	5200.00	2026-03-16 00:03:12.132025
4	2	3	12	2025-11-05	25200.00	2026-03-16 00:03:12.132025
5	2	4	6	2025-12-12	13800.00	2026-03-16 00:03:12.132025
6	2	9	15	2026-01-18	8250.00	2026-03-16 00:03:12.132025
7	3	2	20	2025-10-22	17800.00	2026-03-16 00:03:12.132025
8	3	6	10	2025-12-01	6700.00	2026-03-16 00:03:12.132025
9	3	8	30	2026-01-10	13500.00	2026-03-16 00:03:12.132025
10	4	7	25	2025-09-30	8000.00	2026-03-16 00:03:12.132025
11	4	10	12	2025-11-17	11760.00	2026-03-16 00:03:12.132025
12	4	3	4	2026-01-25	8400.00	2026-03-16 00:03:12.132025
13	5	5	15	2025-08-14	9750.00	2026-03-16 00:03:12.132025
14	5	9	8	2025-12-28	4400.00	2026-03-16 00:03:12.132025
15	5	1	10	2026-02-05	8500.00	2026-03-16 00:03:12.132025
16	6	4	5	2025-11-29	11500.00	2026-03-16 00:03:12.132025
17	6	2	7	2026-01-12	6230.00	2026-03-16 00:03:12.132025
18	7	8	20	2025-12-19	9000.00	2026-03-16 00:03:12.132025
19	7	6	9	2026-02-10	6030.00	2026-03-16 00:03:12.132025
20	8	10	8	2025-10-05	7840.00	2026-03-16 00:03:12.132025
21	8	3	10	2025-12-07	21000.00	2026-03-16 00:03:12.132025
22	9	7	15	2025-11-15	4800.00	2026-03-16 00:03:12.132025
23	9	9	5	2026-01-30	2750.00	2026-03-16 00:03:12.132025
24	10	1	8	2025-12-23	6800.00	2026-03-16 00:03:12.132025
25	10	5	12	2026-02-14	7800.00	2026-03-16 00:03:12.132025
26	11	5	12	2026-03-16	221222.00	2026-03-16 00:15:03.703104
27	11	3	34	2026-03-16	1222222.00	2026-03-16 00:15:18.934911
\.


--
-- Name: partner_types_partner_type_id_seq; Type: SEQUENCE SET; Schema: app; Owner: app
--

SELECT pg_catalog.setval('app.partner_types_partner_type_id_seq', 6, true);


--
-- Name: partners_partner_id_seq; Type: SEQUENCE SET; Schema: app; Owner: app
--

SELECT pg_catalog.setval('app.partners_partner_id_seq', 11, true);


--
-- Name: products_product_id_seq; Type: SEQUENCE SET; Schema: app; Owner: app
--

SELECT pg_catalog.setval('app.products_product_id_seq', 10, true);


--
-- Name: sales_history_sale_id_seq; Type: SEQUENCE SET; Schema: app; Owner: app
--

SELECT pg_catalog.setval('app.sales_history_sale_id_seq', 27, true);


--
-- Name: partner_types partner_types_pkey; Type: CONSTRAINT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.partner_types
    ADD CONSTRAINT partner_types_pkey PRIMARY KEY (partner_type_id);


--
-- Name: partner_types partner_types_type_name_key; Type: CONSTRAINT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.partner_types
    ADD CONSTRAINT partner_types_type_name_key UNIQUE (type_name);


--
-- Name: partners partners_pkey; Type: CONSTRAINT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.partners
    ADD CONSTRAINT partners_pkey PRIMARY KEY (partner_id);


--
-- Name: products products_article_key; Type: CONSTRAINT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.products
    ADD CONSTRAINT products_article_key UNIQUE (article);


--
-- Name: products products_pkey; Type: CONSTRAINT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.products
    ADD CONSTRAINT products_pkey PRIMARY KEY (product_id);


--
-- Name: sales_history sales_history_pkey; Type: CONSTRAINT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.sales_history
    ADD CONSTRAINT sales_history_pkey PRIMARY KEY (sale_id);


--
-- Name: partners fk_partners_partner_types; Type: FK CONSTRAINT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.partners
    ADD CONSTRAINT fk_partners_partner_types FOREIGN KEY (partner_type_id) REFERENCES app.partner_types(partner_type_id) ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- Name: sales_history fk_sales_history_partners; Type: FK CONSTRAINT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.sales_history
    ADD CONSTRAINT fk_sales_history_partners FOREIGN KEY (partner_id) REFERENCES app.partners(partner_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: sales_history fk_sales_history_products; Type: FK CONSTRAINT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.sales_history
    ADD CONSTRAINT fk_sales_history_products FOREIGN KEY (product_id) REFERENCES app.products(product_id) ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- PostgreSQL database dump complete
--

