#!/bin/bash

# Navigate to application folder
cd /home/site/wwwroot/RecSys

# Activate venv (if needed)
source /home/site/wwwroot/venv/bin/activate

# Start Gunicorn with correct app reference
exec gunicorn --bind 0.0.0.0:8000 --timeout 120 --workers 2 RecSys.app:app
