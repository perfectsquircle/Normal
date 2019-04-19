CREATE USER greglow;
CREATE USER azure_pg_admin;
CREATE USER azure_superuser;
CREATE USER toadstool WITH LOGIN;
CREATE DATABASE wide_world_importers_pg;
GRANT ALL PRIVILEGES ON DATABASE wide_world_importers_pg TO toadstool;