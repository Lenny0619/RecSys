#!/bin/bash

# Create log directory
mkdir -p /home/logs

# Enter app directory
cd /home/site/wwwroot/RecSys

# Enable logging
exec > >(tee -a /home/logs/startup.log) 2>&1
echo "=== Startup Script Begin ==="
date

# Install dependencies with explicit paths
echo "Installing dependencies..."
/opt/python/3.9.22/bin/python -m pip install --upgrade pip
/opt/python/3.9.22/bin/python -m pip install --no-cache-dir --user -r requirements.txt

# Verify installation
echo "Installed packages:"
/opt/python/3.9.22/bin/python -m pip list >> /home/logs/startup.log

# Start Gunicorn with correct Flask app reference
echo "Starting Gunicorn..."
exec /opt/python/3.9.22/bin/gunicorn \
  --bind 0.0.0.0:8000 \
  --timeout 900 \
  --preload \
  --workers 1 \
  --worker-class=sync \
  --log-level debug \
  --access-logfile /home/logs/access.log \
  --error-logfile /home/logs/error.log \
  app:app
