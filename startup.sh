#!/bin/bash

# Navigate to the correct directory
cd /home/site/wwwroot

# Activate venv
source venv/bin/activate

# Enable logging
exec > >(tee -a /home/logs/startup.log) 2>&1
echo "=== Startup Script Begin ==="
date

# Install dependencies inside venv
pip install --no-cache-dir -r requirements.txt

# Start Gunicorn
echo "Starting Gunicorn..."
exec gunicorn --bind 0.0.0.0:8000 app:app
