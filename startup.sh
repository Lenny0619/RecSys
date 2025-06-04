#!/bin/bash

# 进入应用目录（关键！）
cd /home/site/wwwroot

# 启用日志记录
exec > >(tee -a /home/logs/startup.log) 2>&1
echo "=== Startup Script Begin ==="
date

# 安装依赖（使用绝对路径的pip）
echo "Installing dependencies..."
/opt/python/3.9.22/bin/python -m pip install --upgrade pip
/opt/python/3.9.22/bin/python -m pip install -r requirements.txt


# 启动Gunicorn（确保在项目根目录运行）
echo "Starting Gunicorn..."
exec /opt/python/3.9.22/bin/gunicorn \
  --bind 0.0.0.0:8000 \
  --timeout 600 \
  --access-logfile - \
  --error-logfile - \
  --workers 1 \
  RecSys.app:app
