Imports System.Data.Entity

Class MainWindow
    Private bd As DemoEntity.Tienda_PrimaryEntities
    Private view As CollectionViewSource

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)

        'Instanciamos el contexto para poder manipular la base de datos
        bd = New DemoEntity.Tienda_PrimaryEntities

        'Cargamos desde la base de datos los datos de la tabla Articulos
        bd.Articulos.Load()
        'Cargamos desde la base de datos los datos de la tabla TiposArticulo
        bd.TiposArticulo.Load()


        'Primera pestaña (Listado de artículos)
        'Configuramos el DataContext para el listado de artículos
        ArticulosListBox.DataContext = bd.Articulos.Local


        'Segunda pestaña (Nuevo artículo)
        'Configuramos el DataContext para el ComboBox de tipos de artículo
        TipoComboBox.DataContext = bd.TiposArticulo.Local


        'Tercera pestaña (Gestión de artículos)
        'Creamos y configuramos la vista que servirá de origen de datos al DataGrid
        view = New CollectionViewSource
        'El origen de la vista es la ObservableCollection de articulos
        view.Source = bd.Articulos.Local
        'El DataContext del DataGrid es la vista
        TablaDataGrid.DataContext = view
        'Añadimos a la vista el manejador del evento Filter para poder hacer busquedas
        AddHandler view.Filter, AddressOf View_Filter

    End Sub

    'Este evento se lanza una vez para cada item de la vista, cada vez que ésta se refresca
    'e.Accepted = True -> el item estará en los resultados del filtro
    Private Sub View_Filter(ByVal sender As System.Object, ByVal e As System.Windows.Data.FilterEventArgs)
        Dim articulo As Articulo = e.Item

        If FiltroTextBox.Text.Trim = "" Then
            e.Accepted = True
        ElseIf articulo.nombre.Contains(FiltroTextBox.Text) Then
            e.Accepted = True
        Else
            e.Accepted = False
        End If

    End Sub

    'La vista se refresca cada vez que queramos aplicar el filtro
    Private Sub FiltroTextBox_TextChanged(sender As Object, e As TextChangedEventArgs)
        view.View.Refresh()
    End Sub

    Private Sub EliminarButton_Click(sender As Object, e As RoutedEventArgs)
        'La vista mantiene el item seleccionado
        Dim articuloSeleccionado As Articulo = view.View.CurrentItem

        If (articuloSeleccionado IsNot Nothing) Then
            'Borramos el item del contexto
            bd.Articulos.Remove(articuloSeleccionado)
            'Trasladamos los cambios a la base de datos
            bd.SaveChanges()
            MessageBox.Show("Artículo eliminado")
        End If

    End Sub

    Private Sub InsertarButton_Click(sender As Object, e As RoutedEventArgs)
        'Añadimos el item al contexto
        bd.Articulos.Add(Me.Resources("nuevo"))
        'Trasladamos los cambios a la base de datos
        bd.SaveChanges()
        MessageBox.Show("Artículo insertado")
    End Sub

End Class
