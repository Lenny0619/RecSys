#!/bin/bash
cd /home/site/wwwroot
source venv/bin/activate
echo "Checking installed packages..."
pip list | grep -E 'Flask|pandas|numpy|scikit-learn|pyodbc'
exec gunicorn --bind 0.0.0.0:8000 app:app
