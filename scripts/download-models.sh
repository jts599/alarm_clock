#!/bin/bash

# Model download script for alarm clock project
# Usage: ./scripts/download-models.sh

set -e

MODELS_DIR="/workspaces/alarm_clock/models"
mkdir -p "$MODELS_DIR"

echo "🤖 Downloading TinyLlama ONNX model..."

# TinyLlama model download
TINYLLAMA_URL="https://huggingface.co/microsoft/TinyLlama-1.1B-Chat-v1.0-onnx/resolve/main/model.onnx"
TINYLLAMA_PATH="$MODELS_DIR/tinyllama-1.1b-chat.onnx"

if [ ! -f "$TINYLLAMA_PATH" ]; then
    echo "Downloading TinyLlama model (~400MB)..."
    curl -L -o "$TINYLLAMA_PATH" "$TINYLLAMA_URL"
    echo "✅ TinyLlama model downloaded"
else
    echo "✅ TinyLlama model already exists"
fi

echo "🎉 Model ready!"