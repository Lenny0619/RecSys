#!/bin/bash

# Create log directory
mkdir -p /home/logs

# Navigate to the application directory
cd /home/site/wwwroot/

# Enable logging
exec > >(tee -a /home/logs/startup.log) 2>&1
echo "=== Startup Script Begin ==="
date

# Check for requirements.txt and install dependencies
if [ -f requirements.txt ]; then
    echo "Installing dependencies from root folder..."
    /opt/python/3.9.22/bin/python -m pip install --no-cache-dir --user -r requirements.txt
elif [ -f RecSys/requirements.txt ]; then
    echo "Installing dependencies from RecSys folder..."
    cd RecSys
    /opt/python/3.9.22/bin/python -m pip install --no-cache-dir --user -r requirements.txt
else
    echo "ERROR: requirements.txt not found! Deployment failed."
    exit 1
fi

# Verify installation of key packages
echo "Checking installed packages..."
/opt/python/3.9.22/bin/python -m pip list | tee /home/logs/packages.log

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