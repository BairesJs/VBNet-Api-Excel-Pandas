#!/usr/bin/env python3
from flask import Flask, jsonify, request, render_template
import pandas as pd

app = Flask(__name__)

def guardar_datos_en_excel(datos):
    try:
        df = pd.read_excel('db.xlsx')
    except FileNotFoundError:
        df = pd.DataFrame()

    nuevo_registro = pd.DataFrame(datos)
    df = pd.concat([df, nuevo_registro], ignore_index=True)
    df.to_excel('db.xlsx', index=False)

@app.route('/data', methods=['GET'])
def get_data():
    try:
        df = pd.read_excel('db.xlsx')
        data = df.to_dict(orient='records')
        return jsonify(data), 200
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/form', methods=['GET'])
def show_form():
    return render_template('form.html')

@app.route('/form', methods=['POST'])
def handle_form():
    try:
        request_data = request.form.to_dict(flat=False)
        print("Datos recibidos del formulario:", request_data)  # Mensaje de depuración
        guardar_datos_en_excel(request_data)
        return jsonify({'message': 'Data received and saved successfully'}), 200
    except Exception as e:
        return jsonify({'error': str(e)}), 500
    
@app.route('/update', methods=['POST'])
def handle_update():
    try:
        request_data = request.form.to_dict(flat=False)
        print("Datos recibidos para actualizar:", request_data)  # Mensaje de depuración
        guardar_datos_en_excel(request_data)
        return jsonify({'message': 'Data updated successfully'}), 200
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/data/<int:id>', methods=['DELETE'])
def delete_data(id):
    try:
        df = pd.read_excel('db.xlsx')
        df = df[df['id'] != id]  # Filtrar el DataFrame para excluir el registro con el ID dado
        df.to_excel('db.xlsx', index=False)  # Guardar el DataFrame actualizado en el archivo Excel
        return jsonify({'message': f'Data with ID {id} deleted successfully'}), 200
    except Exception as e:
        return jsonify({'error': str(e)}), 500



if __name__ == '__main__':
    app.run(debug=True)
