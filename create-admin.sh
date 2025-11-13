#!/bin/bash
# Script to create an admin account

echo "Creating admin account..."
curl -X POST http://localhost:5004/api/AdminSeed/create-initial-admin \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123","email":"admin@example.com"}'

echo ""
echo "If you see 'An admin already exists', you can use the login page instead."
echo "Login URL: http://localhost:5004/admin/login.html"


