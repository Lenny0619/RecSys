#!/bin/bash

# Create log directory
mkdir -p /home/logs

# Enter app directory
cd /home/site/wwwroot

# Enable logging
exec > >(tee -a /home/logs/startup.log) 2>&1
echo "=== Startup Script Begin ==="
date

# Install dependencies (critical for Azure Free Tier!)
echo "Installing dependencies..."
/opt/python/3.9.22/bin/python -m pip install --upgrade pip
/opt/python/3.9.22/bin/python -m pip install --no-cache-dir -r requirements.txt

# Start Gunicorn
echo "Starting Gunicorn..."
exec /opt/python/3.9.22/bin/gunicorn \
  --bind 0.0.0.0:8000 \
  --timeout 600 \
  --access-logfile - \
  --error-logfile - \
  --workers 1 \
  RecSys.app:app