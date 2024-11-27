SET statement_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;
COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';

SET search_path = public, pg_catalog;
SET default_tablespace = '';
SET default_with_oids = false;

CREATE SEQUENCE categories_seq;
CREATE TABLE "Categories" (
    "CategoryID" smallint DEFAULT nextval('categories_seq') NOT NULL,
    "CategoryName" character varying(15) NOT NULL,
    "Description" text,
    "Picture" bytea
);

ALTER TABLE public."Categories" OWNER TO postgres;

CREATE TABLE "CustomerCustomerDemo" (
    "CustomerID" bpchar NOT NULL,
    "CustomerTypeID" bpchar NOT NULL
);

ALTER TABLE public."CustomerCustomerDemo" OWNER TO postgres;

CREATE TABLE "CustomerDemographics" (
    "CustomerTypeID" bpchar NOT NULL,
    "CustomerDesc" text
);

ALTER TABLE public."CustomerDemographics" OWNER TO postgres;

CREATE TABLE "Customers" (
    "CustomerID" bpchar NOT NULL,
    "CompanyName" character varying(40) NOT NULL,
    "ContactName" character varying(30),
    "ContactTitle" character varying(30),
    "Address" character varying(60),
    "City" character varying(15),
    "Region" character varying(15),
    "PostalCode" character varying(10),
    "Country" character varying(15),
    "Phone" character varying(24),
    "Fax" character varying(24)
);

ALTER TABLE public."Customers" OWNER TO postgres;

CREATE SEQUENCE employees_seq;

CREATE TABLE "Employees" (
    "EmployeeID" smallint DEFAULT nextval('employees_seq') NOT NULL,
    "LastName" character varying(20) NOT NULL,
    "FirstName" character varying(10) NOT NULL,
    "Title" character varying(30),
    "TitleOfCourtesy" character varying(25),
    "BirthDate" date,
    "HireDate" date,
    "Address" character varying(60),
    "City" character varying(15),
    "Region" character varying(15),
    "PostalCode" character varying(10),
    "Country" character varying(15),
    "HomePhone" character varying(24),
    "Extension" character varying(4),
    "Photo" bytea,
    "Notes" text,
    "ReportsTo" smallint,
    "PhotoPath" character varying(255)
);


ALTER TABLE public."Employees" OWNER TO postgres;

CREATE TABLE "EmployeeTerritories" (
    "EmployeeID" smallint NOT NULL,
    "TerritoryID" character varying(20) NOT NULL
);


ALTER TABLE public."EmployeeTerritories" OWNER TO postgres;

CREATE TABLE "Order Details" (
    "OrderID" smallint NOT NULL,
    "ProductID" smallint NOT NULL,
    "UnitPrice" real NOT NULL,
    "Quantity" smallint NOT NULL,
    "Discount" real NOT NULL
);


ALTER TABLE public."Order Details" OWNER TO postgres;

CREATE SEQUENCE orders_seq;

CREATE TABLE "Orders" (
    "OrderID" smallint DEFAULT nextval('orders_seq') NOT NULL,
    "CustomerID" bpchar,
    "EmployeeID" smallint,
    "OrderDate" date,
    "RequiredDate" date,
    "ShippedDate" date,
    "ShipVia" smallint,
    "Freight" real,
    "ShipName" character varying(40),
    "ShipAddress" character varying(60),
    "ShipCity" character varying(15),
    "ShipRegion" character varying(15),
    "ShipPostalCode" character varying(10),
    "ShipCountry" character varying(15)
);


ALTER TABLE public."Orders" OWNER TO postgres;

CREATE SEQUENCE products_seq;

CREATE TABLE "Products" (
    "ProductID" smallint DEFAULT nextval('products_seq') NOT NULL,
    "ProductName" character varying(40) NOT NULL,
    "SupplierID" smallint,
    "CategoryID" smallint,
    "QuantityPerUnit" character varying(20),
    "UnitPrice" real,
    "UnitsInStock" smallint,
    "UnitsOnOrder" smallint,
    "ReorderLevel" smallint,
    "Discontinued" boolean NOT NULL
);


ALTER TABLE public."Products" OWNER TO postgres;

CREATE SEQUENCE region_seq;

CREATE TABLE "Region" (
    "RegionID" smallint DEFAULT nextval('region_seq') NOT NULL,
    "RegionDescription" bpchar NOT NULL
);

ALTER TABLE public."Region" OWNER TO postgres;

CREATE SEQUENCE shippers_seq;

CREATE TABLE "Shippers" (
    "ShipperID" smallint DEFAULT nextval('shippers_seq') NOT NULL,
    "CompanyName" character varying(40) NOT NULL,
    "Phone" character varying(24)
);


ALTER TABLE public."Shippers" OWNER TO postgres;

CREATE SEQUENCE suppliers_seq;

CREATE TABLE "Suppliers" (
    "SupplierID" smallint DEFAULT nextval('suppliers_seq') NOT NULL,
    "CompanyName" character varying(40) NOT NULL,
    "ContactName" character varying(30),
    "ContactTitle" character varying(30),
    "Address" character varying(60),
    "City" character varying(15),
    "Region" character varying(15),
    "PostalCode" character varying(10),
    "Country" character varying(15),
    "Phone" character varying(24),
    "Fax" character varying(24),
    "HomePage" text
);


ALTER TABLE public."Suppliers" OWNER TO postgres;

CREATE TABLE "Territories" (
    "TerritoryID" character varying(20) NOT NULL,
    "TerritoryDescription" bpchar NOT NULL,
    "RegionID" smallint NOT NULL
);

ALTER TABLE public."Territories" OWNER TO postgres;

CREATE SEQUENCE usstates_seq;

CREATE TABLE "UsStates" (
    "StateID" smallint DEFAULT nextval('usstates_seq') NOT NULL,
    "StateName" character varying(100),
    "StateAbbr" character varying(2),
    "StateRegion" character varying(50)
);


ALTER TABLE public."UsStates" OWNER TO postgres;



ALTER TABLE ONLY "Categories"
    ADD CONSTRAINT "PK_Categories" PRIMARY KEY ("CategoryID");

ALTER TABLE ONLY "CustomerCustomerDemo"
    ADD CONSTRAINT "PK_CustomerCustomerDemo" PRIMARY KEY ("CustomerID", "CustomerTypeID");

ALTER TABLE ONLY "CustomerDemographics"
    ADD CONSTRAINT "PK_CustomerDemographics" PRIMARY KEY ("CustomerTypeID");

ALTER TABLE ONLY "Customers"
    ADD CONSTRAINT "PK_Customers" PRIMARY KEY ("CustomerID");

ALTER TABLE ONLY "Employees"
    ADD CONSTRAINT "PK_Employees" PRIMARY KEY ("EmployeeID");

ALTER TABLE ONLY "EmployeeTerritories"
    ADD CONSTRAINT "PK_EmployeeTerritories" PRIMARY KEY ("EmployeeID", "TerritoryID");

ALTER TABLE ONLY "Order Details"
    ADD CONSTRAINT "PK_Order Details" PRIMARY KEY ("OrderID", "ProductID");

ALTER TABLE ONLY "Orders"
    ADD CONSTRAINT "PK_Orders" PRIMARY KEY ("OrderID");

ALTER TABLE ONLY "Products"
    ADD CONSTRAINT "PK_Products" PRIMARY KEY ("ProductID");

ALTER TABLE ONLY "Region"
    ADD CONSTRAINT "PK_Region" PRIMARY KEY ("RegionID");

ALTER TABLE ONLY "Shippers"
    ADD CONSTRAINT "PK_Shippers" PRIMARY KEY ("ShipperID");

ALTER TABLE ONLY "Suppliers"
    ADD CONSTRAINT "PK_Suppliers" PRIMARY KEY ("SupplierID");

ALTER TABLE ONLY "Territories"
    ADD CONSTRAINT "PK_Territories" PRIMARY KEY ("TerritoryID");

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;