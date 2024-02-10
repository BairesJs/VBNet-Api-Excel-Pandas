Imports System.Net.Http
Imports System.Data
Imports Newtonsoft.Json.JsonConvert
Imports Newtonsoft.Json
Imports System.Net
Imports System.Text
Imports System.IO


Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        '/////////////////////////////////////////////////////
        TextBox1.Enabled = False
        TextBox2.Enabled = False
        TextBox3.Enabled = False
        TextBox4.Enabled = False
        Button1.Enabled = False
        TextBox5.Visible = False

        '/////////////////////////////////////////////////////
        Try
            ' Realizar la solicitud GET para obtener los datos más recientes
            Dim getDataUrl As String = "http://127.0.0.1:5000/data"

            Using httpClient As New HttpClient()
                Dim getDataResponse As HttpResponseMessage = httpClient.GetAsync(getDataUrl).Result

                If getDataResponse.IsSuccessStatusCode Then
                    Dim responseData As String = getDataResponse.Content.ReadAsStringAsync().Result
                    Dim dataList As List(Of Dictionary(Of String, String)) = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, String)))(responseData)

                    ' Verificar si hay datos en dataList
                    If dataList.Count > 0 Then
                        ' Limpiar el DataGridView
                        DataGridView1.Rows.Clear()
                        DataGridView1.Columns.Clear()

                        ' Obtener las claves del primer diccionario en dataList
                        Dim keys = dataList(0).Keys.ToList()

                        ' Crear DataTable y agregar columnas
                        Dim dataTable As New DataTable()
                        For Each key As String In keys
                            dataTable.Columns.Add(key)
                        Next

                        ' Agregar filas al DataTable
                        For Each dataItem As Dictionary(Of String, String) In dataList
                            Dim row As DataRow = dataTable.NewRow()

                            ' Agregar valores de las celdas a la fila
                            For Each key As String In keys
                                row(key) = dataItem(key)
                            Next

                            ' Agregar la fila al DataTable
                            dataTable.Rows.Add(row)
                        Next

                        ' Actualizar el DataSource del DataGridView en el hilo de la interfaz de usuario
                        DataGridView1.Invoke(Sub()
                                                 DataGridView1.DataSource = dataTable
                                                 ' Mover las columnas según lo necesario
                                                 If DataGridView1.Columns.Count > 0 Then
                                                     ' Mover la columna "Nombre" al principio
                                                     If DataGridView1.Columns.Contains("id") Then
                                                         DataGridView1.Columns("id").DisplayIndex = 0
                                                     End If

                                                     ' Mover la columna "Edad" después de la columna "Nombre"
                                                     If DataGridView1.Columns.Contains("nombre") Then
                                                         DataGridView1.Columns("nombre").DisplayIndex = 1
                                                     End If

                                                     ' Mover la columna "ID" al final
                                                     If DataGridView1.Columns.Contains("direccion") Then
                                                         DataGridView1.Columns("direccion").DisplayIndex = 2
                                                     End If

                                                     If DataGridView1.Columns.Contains("telefono") Then
                                                         DataGridView1.Columns("telefono").DisplayIndex = 3
                                                     End If

                                                     If DataGridView1.Columns.Contains("mail") Then
                                                         DataGridView1.Columns("mail").DisplayIndex = 4
                                                     End If
                                                 End If
                                             End Sub)
                    Else
                        MessageBox.Show("No se encontraron datos para actualizar.")
                    End If
                Else
                    MessageBox.Show("Error al obtener los datos para actualizar. Código de estado: " & getDataResponse.StatusCode)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error al actualizar los datos del DataGridView: " & ex.Message)
        End Try

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            ' Realizar la solicitud GET para obtener los datos más recientes
            Dim getDataUrl As String = "http://127.0.0.1:5000/data"

            Using httpClient As New HttpClient()
                Dim getDataResponse As HttpResponseMessage = httpClient.GetAsync(getDataUrl).Result ' Síncrono

                If getDataResponse.IsSuccessStatusCode Then
                    Dim responseData As String = getDataResponse.Content.ReadAsStringAsync().Result ' Síncrono
                    Dim dataList As List(Of Dictionary(Of String, String)) = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, String)))(responseData)

                    ' Extraer el último ID
                    If dataList.Any() Then
                        Dim lastIdString As String = dataList.Last()("id") ' Suponiendo que el ID es una cadena
                        Dim lastId As Integer
                        If Integer.TryParse(lastIdString, lastId) Then
                            ' Sumarle 1 al último ID
                            Dim siguienteId As Integer = lastId + 1

                            ' Escribir el siguiente ID en textBox5
                            TextBox5.Text = siguienteId.ToString()
                        Else
                            ' Manejar el caso en el que el último ID no sea un número válido
                            TextBox5.Text = "1" ' Por ejemplo, podrías establecer el valor predeterminado a 1
                        End If
                    Else
                        ' Manejar el caso en el que no haya datos en la lista
                        TextBox5.Text = "1" ' Por ejemplo, podrías establecer el valor predeterminado a 1
                    End If
                Else
                    ' Manejar el caso en el que la solicitud no sea exitosa
                    TextBox5.Text = "1" ' Por ejemplo, podrías establecer el valor predeterminado a 1
                End If
            End Using
        Catch ex As Exception
            ' Manejar cualquier excepción que pueda ocurrir durante la solicitud
            Console.WriteLine("Error al obtener los datos: " & ex.Message)
        End Try

        TextBox1.Enabled = True
        TextBox2.Enabled = True
        TextBox3.Enabled = True
        TextBox4.Enabled = True
        TextBox1.Focus()
        Button1.Enabled = True
        Button2.Enabled = False


    End Sub

    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs) Handles TextBox6.TextChanged
        Try
            Dim nombreBuscado As String = TextBox6.Text.Trim() ' Obtener el texto ingresado en textBox6 para la búsqueda por nombre

            ' Realizar la solicitud GET para obtener los datos más recientes
            Dim getDataUrl As String = "http://127.0.0.1:5000/data"

            Using httpClient As New HttpClient()
                Dim getDataResponse As HttpResponseMessage = httpClient.GetAsync(getDataUrl).Result

                If getDataResponse.IsSuccessStatusCode Then
                    Dim responseData As String = getDataResponse.Content.ReadAsStringAsync().Result
                    Dim dataList As List(Of Dictionary(Of String, String)) = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, String)))(responseData)

                    ' Filtrar los datos por nombre en tiempo real mientras el usuario escribe
                    Dim resultadosBusqueda As List(Of Dictionary(Of String, String)) = dataList _
                .Where(Function(d) d.ContainsKey("nombre") AndAlso d("nombre").IndexOf(nombreBuscado, StringComparison.OrdinalIgnoreCase) >= 0) _
                .ToList()

                    ' Crear DataTable y agregar columnas
                    Dim dataTable As New DataTable()
                    If resultadosBusqueda.Count > 0 Then ' Usar los resultados de la búsqueda en lugar de los datos originales
                        For Each key In resultadosBusqueda(0).Keys
                            dataTable.Columns.Add(key)
                        Next

                        ' Agregar filas al DataTable
                        For Each item In resultadosBusqueda
                            Dim row As DataRow = dataTable.NewRow()
                            For Each key In item.Keys
                                row(key) = item(key)
                            Next
                            dataTable.Rows.Add(row)
                        Next



                        ' Actualizar el DataSource del DataGridView en el hilo de la interfaz de usuario
                        DataGridView1.Invoke(Sub()
                                                 DataGridView1.DataSource = dataTable
                                                 ' Mover las columnas según lo necesario
                                                 If DataGridView1.Columns.Count > 0 Then
                                                     ' Mover la columna "Nombre" al principio
                                                     If DataGridView1.Columns.Contains("id") Then
                                                         DataGridView1.Columns("id").DisplayIndex = 0
                                                     End If

                                                     ' Mover la columna "Edad" después de la columna "Nombre"
                                                     If DataGridView1.Columns.Contains("nombre") Then
                                                         DataGridView1.Columns("nombre").DisplayIndex = 1
                                                     End If

                                                     ' Mover la columna "ID" al final
                                                     If DataGridView1.Columns.Contains("direccion") Then
                                                         DataGridView1.Columns("direccion").DisplayIndex = 2
                                                     End If

                                                     If DataGridView1.Columns.Contains("telefono") Then
                                                         DataGridView1.Columns("telefono").DisplayIndex = 3
                                                     End If

                                                     If DataGridView1.Columns.Contains("mail") Then
                                                         DataGridView1.Columns("mail").DisplayIndex = 4
                                                     End If
                                                 End If
                                             End Sub)

                        ' Resto del código para ajustar las columnas del DataGridView...
                    Else
                        DataGridView1.DataSource = Nothing ' Limpiar el DataGridView si no se encuentran resultados
                    End If
                Else
                    MessageBox.Show("Error al obtener los datos. Código de estado: " & getDataResponse.StatusCode)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error al procesar la búsqueda: " & ex.Message)
        End Try

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Dim id As Integer = Integer.Parse(TextBox5.Text)
            Dim nombre As String = TextBox1.Text
            Dim direccion As String = TextBox2.Text
            Dim telefono As String = TextBox3.Text
            Dim email As String = TextBox4.Text

            Dim postData As String = "id=" & id & "&nombre=" & nombre & "&direccion=" & direccion & "&telefono=" & telefono & "&email=" & email
            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)

            Dim postUrl As String = "http://127.0.0.1:5000/form"

            Dim request As HttpWebRequest = DirectCast(WebRequest.Create(postUrl), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/x-www-form-urlencoded"
            request.ContentLength = byteArray.Length

            Using stream = request.GetRequestStream()
                stream.Write(byteArray, 0, byteArray.Length)
            End Using

            ' Realizar la solicitud POST de forma síncrona
            Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
            Dim responseText As String
            Using reader As New System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8)
                responseText = reader.ReadToEnd()
            End Using
            response.Close()

            'MessageBox.Show("Datos enviados correctamente.")
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try

        '/////////////////////////////////////////////////////
        Try
            ' Realizar la solicitud GET para obtener los datos más recientes
            Dim getDataUrl As String = "http://127.0.0.1:5000/data"

            Using httpClient As New HttpClient()
                Dim getDataResponse As HttpResponseMessage = httpClient.GetAsync(getDataUrl).Result

                If getDataResponse.IsSuccessStatusCode Then
                    Dim responseData As String = getDataResponse.Content.ReadAsStringAsync().Result
                    Dim dataList As List(Of Dictionary(Of String, String)) = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, String)))(responseData)

                    ' Crear DataTable y agregar columnas
                    Dim dataTable As New DataTable()
                    If dataList.Count > 0 Then
                        For Each key As String In dataList(0).Keys
                            dataTable.Columns.Add(key)
                        Next

                        ' Agregar filas al DataTable
                        For Each data As Dictionary(Of String, String) In dataList
                            Dim row As DataRow = dataTable.NewRow()
                            For Each key As String In data.Keys
                                row(key) = data(key)
                            Next
                            dataTable.Rows.Add(row)
                        Next

                        ' Actualizar el DataSource del DataGridView en el hilo de la interfaz de usuario
                        DataGridView1.Invoke(Sub()
                                                 DataGridView1.DataSource = dataTable
                                                 ' Mover las columnas según lo necesario
                                                 If DataGridView1.Columns.Count > 0 Then
                                                     ' Mover la columna "Nombre" al principio
                                                     If DataGridView1.Columns.Contains("id") Then
                                                         DataGridView1.Columns("id").DisplayIndex = 0
                                                     End If

                                                     ' Mover la columna "Edad" después de la columna "Nombre"
                                                     If DataGridView1.Columns.Contains("nombre") Then
                                                         DataGridView1.Columns("nombre").DisplayIndex = 1
                                                     End If

                                                     ' Mover la columna "ID" al final
                                                     If DataGridView1.Columns.Contains("direccion") Then
                                                         DataGridView1.Columns("direccion").DisplayIndex = 2
                                                     End If

                                                     If DataGridView1.Columns.Contains("telefono") Then
                                                         DataGridView1.Columns("telefono").DisplayIndex = 3
                                                     End If

                                                     If DataGridView1.Columns.Contains("mail") Then
                                                         DataGridView1.Columns("mail").DisplayIndex = 4
                                                     End If
                                                 End If
                                             End Sub)
                    Else
                        MessageBox.Show("No se encontraron datos para actualizar.")
                    End If
                Else
                    MessageBox.Show("Error al obtener los datos para actualizar. Código de estado: " & getDataResponse.StatusCode)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error al actualizar los datos del DataGridView: " & ex.Message)
        End Try

        TextBox1.Text = ""
        TextBox2.Text = ""
        TextBox3.Text = ""
        TextBox4.Text = ""

        TextBox1.Enabled = False
        TextBox2.Enabled = False
        TextBox3.Enabled = False
        TextBox4.Enabled = False
        Button1.Enabled = False
        Button2.Enabled = True

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Try
            Dim idABorrar As Integer
            If Not Integer.TryParse(TextBox7.Text.Trim(), idABorrar) Then
                MessageBox.Show("Ingrese un ID válido para el registro a borrar.")
                Return
            End If

            Dim urlBorrar As String = $"http://127.0.0.1:5000/data/{idABorrar}"

            Dim request As WebRequest = WebRequest.Create(urlBorrar)
            request.Method = "DELETE"

            Dim response As WebResponse = request.GetResponse()
            Dim dataStream As Stream = response.GetResponseStream()
            Dim reader As New StreamReader(dataStream)
            Dim responseFromServer As String = reader.ReadToEnd()

            MessageBox.Show($"Registro con ID {idABorrar} borrado exitosamente.")

            reader.Close()
            response.Close()

            ' Actualizar el DataGridView u otra lógica necesaria después de borrar el registro...
        Catch ex As Exception
            MessageBox.Show("Error al borrar el registro: " & ex.Message)
        End Try

        '/////////////////////////////////////////////////////
        Try
            ' Realizar la solicitud GET para obtener los datos más recientes
            Dim getDataUrl As String = "http://127.0.0.1:5000/data"

            Using httpClient As New HttpClient()
                Dim getDataResponse As HttpResponseMessage = httpClient.GetAsync(getDataUrl).Result

                If getDataResponse.IsSuccessStatusCode Then
                    Dim responseData As String = getDataResponse.Content.ReadAsStringAsync().Result
                    Dim dataList As List(Of Dictionary(Of String, String)) = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, String)))(responseData)

                    ' Crear DataTable y agregar columnas
                    Dim dataTable As New DataTable()
                    If dataList.Count > 0 Then
                        For Each key As String In dataList(0).Keys
                            dataTable.Columns.Add(key)
                        Next

                        ' Agregar filas al DataTable
                        For Each data As Dictionary(Of String, String) In dataList
                            Dim row As DataRow = dataTable.NewRow()
                            For Each key As String In data.Keys
                                row(key) = data(key)
                            Next
                            dataTable.Rows.Add(row)
                        Next

                        ' Actualizar el DataSource del DataGridView en el hilo de la interfaz de usuario
                        DataGridView1.Invoke(Sub()
                                                 DataGridView1.DataSource = dataTable
                                             End Sub)

                        If DataGridView1.Columns.Count > 0 Then
                            ' Mover las columnas según lo necesario
                            If DataGridView1.Columns.Contains("id") Then
                                DataGridView1.Columns("id").DisplayIndex = 0
                            End If

                            If DataGridView1.Columns.Contains("nombre") Then
                                DataGridView1.Columns("nombre").DisplayIndex = 1
                            End If

                            If DataGridView1.Columns.Contains("direccion") Then
                                DataGridView1.Columns("direccion").DisplayIndex = 2
                            End If

                            If DataGridView1.Columns.Contains("telefono") Then
                                DataGridView1.Columns("telefono").DisplayIndex = 3
                            End If

                            If DataGridView1.Columns.Contains("mail") Then
                                DataGridView1.Columns("mail").DisplayIndex = 4
                            End If
                        End If
                    Else
                        MessageBox.Show("No se encontraron datos para actualizar.")
                    End If
                Else
                    MessageBox.Show("Error al obtener los datos para actualizar. Código de estado: " & getDataResponse.StatusCode)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error al actualizar los datos del DataGridView: " & ex.Message)
        End Try

        TextBox7.Text = ""

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Application.Exit()
    End Sub

    Private Sub TextBox8_TextChanged(sender As Object, e As EventArgs) Handles TextBox8.TextChanged
        Try
            Dim nombreBuscado As String = TextBox8.Text.Trim() ' Obtener el texto ingresado en textBox6 para la búsqueda por nombre

            ' Realizar la solicitud GET para obtener los datos más recientes
            Dim getDataUrl As String = "http://127.0.0.1:5000/data"

            Using httpClient As New HttpClient()
                Dim getDataResponse As HttpResponseMessage = httpClient.GetAsync(getDataUrl).Result

                If getDataResponse.IsSuccessStatusCode Then
                    Dim responseData As String = getDataResponse.Content.ReadAsStringAsync().Result
                    Dim dataList As List(Of Dictionary(Of String, String)) = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, String)))(responseData)

                    ' Filtrar los datos por nombre en tiempo real mientras el usuario escribe
                    Dim resultadosBusqueda As List(Of Dictionary(Of String, String)) = dataList _
                .Where(Function(d) d.ContainsKey("telefono") AndAlso d("telefono").IndexOf(nombreBuscado, StringComparison.OrdinalIgnoreCase) >= 0) _
                .ToList()

                    ' Crear DataTable y agregar columnas
                    Dim dataTable As New DataTable()
                    If resultadosBusqueda.Count > 0 Then ' Usar los resultados de la búsqueda en lugar de los datos originales
                        For Each key In resultadosBusqueda(0).Keys
                            dataTable.Columns.Add(key)
                        Next

                        ' Agregar filas al DataTable
                        For Each item In resultadosBusqueda
                            Dim row As DataRow = dataTable.NewRow()
                            For Each key In item.Keys
                                row(key) = item(key)
                            Next
                            dataTable.Rows.Add(row)
                        Next



                        ' Actualizar el DataSource del DataGridView en el hilo de la interfaz de usuario
                        DataGridView1.Invoke(Sub()
                                                 DataGridView1.DataSource = dataTable
                                                 ' Mover las columnas según lo necesario
                                                 If DataGridView1.Columns.Count > 0 Then
                                                     ' Mover la columna "Nombre" al principio
                                                     If DataGridView1.Columns.Contains("id") Then
                                                         DataGridView1.Columns("id").DisplayIndex = 0
                                                     End If

                                                     ' Mover la columna "Edad" después de la columna "Nombre"
                                                     If DataGridView1.Columns.Contains("nombre") Then
                                                         DataGridView1.Columns("nombre").DisplayIndex = 1
                                                     End If

                                                     ' Mover la columna "ID" al final
                                                     If DataGridView1.Columns.Contains("direccion") Then
                                                         DataGridView1.Columns("direccion").DisplayIndex = 2
                                                     End If

                                                     If DataGridView1.Columns.Contains("telefono") Then
                                                         DataGridView1.Columns("telefono").DisplayIndex = 3
                                                     End If

                                                     If DataGridView1.Columns.Contains("mail") Then
                                                         DataGridView1.Columns("mail").DisplayIndex = 4
                                                     End If
                                                 End If
                                             End Sub)

                        ' Resto del código para ajustar las columnas del DataGridView...
                    Else
                        DataGridView1.DataSource = Nothing ' Limpiar el DataGridView si no se encuentran resultados
                    End If
                Else
                    MessageBox.Show("Error al obtener los datos. Código de estado: " & getDataResponse.StatusCode)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error al procesar la búsqueda: " & ex.Message)
        End Try

    End Sub

    Private Sub TextBox9_TextChanged(sender As Object, e As EventArgs) Handles TextBox9.TextChanged
        Try
            Dim nombreBuscado As String = TextBox9.Text.Trim() ' Obtener el texto ingresado en textBox6 para la búsqueda por nombre

            ' Realizar la solicitud GET para obtener los datos más recientes
            Dim getDataUrl As String = "http://127.0.0.1:5000/data"

            Using httpClient As New HttpClient()
                Dim getDataResponse As HttpResponseMessage = httpClient.GetAsync(getDataUrl).Result

                If getDataResponse.IsSuccessStatusCode Then
                    Dim responseData As String = getDataResponse.Content.ReadAsStringAsync().Result
                    Dim dataList As List(Of Dictionary(Of String, String)) = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, String)))(responseData)

                    ' Filtrar los datos por nombre en tiempo real mientras el usuario escribe
                    Dim resultadosBusqueda As List(Of Dictionary(Of String, String)) = dataList _
                .Where(Function(d) d.ContainsKey("email") AndAlso d("email").IndexOf(nombreBuscado, StringComparison.OrdinalIgnoreCase) >= 0) _
                .ToList()

                    ' Crear DataTable y agregar columnas
                    Dim dataTable As New DataTable()
                    If resultadosBusqueda.Count > 0 Then ' Usar los resultados de la búsqueda en lugar de los datos originales
                        For Each key In resultadosBusqueda(0).Keys
                            dataTable.Columns.Add(key)
                        Next

                        ' Agregar filas al DataTable
                        For Each item In resultadosBusqueda
                            Dim row As DataRow = dataTable.NewRow()
                            For Each key In item.Keys
                                row(key) = item(key)
                            Next
                            dataTable.Rows.Add(row)
                        Next



                        ' Actualizar el DataSource del DataGridView en el hilo de la interfaz de usuario
                        DataGridView1.Invoke(Sub()
                                                 DataGridView1.DataSource = dataTable
                                                 ' Mover las columnas según lo necesario
                                                 If DataGridView1.Columns.Count > 0 Then
                                                     ' Mover la columna "Nombre" al principio
                                                     If DataGridView1.Columns.Contains("id") Then
                                                         DataGridView1.Columns("id").DisplayIndex = 0
                                                     End If

                                                     ' Mover la columna "Edad" después de la columna "Nombre"
                                                     If DataGridView1.Columns.Contains("nombre") Then
                                                         DataGridView1.Columns("nombre").DisplayIndex = 1
                                                     End If

                                                     ' Mover la columna "ID" al final
                                                     If DataGridView1.Columns.Contains("direccion") Then
                                                         DataGridView1.Columns("direccion").DisplayIndex = 2
                                                     End If

                                                     If DataGridView1.Columns.Contains("telefono") Then
                                                         DataGridView1.Columns("telefono").DisplayIndex = 3
                                                     End If

                                                     If DataGridView1.Columns.Contains("mail") Then
                                                         DataGridView1.Columns("mail").DisplayIndex = 4
                                                     End If
                                                 End If
                                             End Sub)

                        ' Resto del código para ajustar las columnas del DataGridView...
                    Else
                        DataGridView1.DataSource = Nothing ' Limpiar el DataGridView si no se encuentran resultados
                    End If
                Else
                    MessageBox.Show("Error al obtener los datos. Código de estado: " & getDataResponse.StatusCode)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error al procesar la búsqueda: " & ex.Message)
        End Try

    End Sub
End Class
