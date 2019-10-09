CREATE USER greglow;
CREATE USER azure_pg_admin;
CREATE USER azure_superuser;
CREATE USER normal WITH LOGIN;
ALTER USER normal WITH PASSWORD 'normal';
CREATE DATABASE wide_world_importers_pg;

GRANT azure_pg_admin TO greglow;
GRANT azure_pg_admin TO normal;
GRANT greglow TO normal;
