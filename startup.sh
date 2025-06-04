#!/bin/bash

# 创建日志目录
mkdir -p /home/logs

# 进入应用目录
cd /home/site/wwwroot

# 启用日志记录
exec > >(tee -a /home/logs/startup.log) 2>&1
echo "=== Startup Script Begin ==="
date

# 仅启动Gunicorn（依赖已在部署前安装）
echo "Starting Gunicorn..."
exec /opt/python/3.9.22/bin/gunicorn \
  --bind 0.0.0.0:8000 \
  --timeout 600 \
  --access-logfile - \
  --error-logfile - \
  --workers 1 \
  RecSys.app:app