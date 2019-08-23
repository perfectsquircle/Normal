CREATE USER greglow;
CREATE USER azure_pg_admin;
CREATE USER azure_superuser;
CREATE USER toadstool WITH LOGIN;
ALTER USER toadstool WITH PASSWORD 'toadstool';
CREATE DATABASE wide_world_importers_pg;

GRANT azure_pg_admin TO greglow;
GRANT azure_pg_admin TO toadstool;
GRANT greglow TO toadstool;
