using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clasesemana4
{
    public partial class Form1 : Form
    {
        private DataTable dtCategorias = new DataTable();

        public Form1()
        {
            InitializeComponent();
            this.iconButton3.Click += new EventHandler(iconButton3_Click);
            CargarCategoriasParaCboCat();
        }

        void CargarProductos()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(Properties.Settings.Default.cc))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("pa_listaProductos", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (!dr.HasRows)
                    {
                        MessageBox.Show("No se encontraron productos.");
                        return;
                    }

                    dataGridView1.Rows.Clear();
                    int count = 0;
                    while (dr.Read())
                    {
                      
                        var debugValues = Enumerable.Range(0, dr.FieldCount)
                                            .Select(i => dr.GetValue(i))
                                            .ToList();
                        Debug.WriteLine("Valores de fila: " + string.Join(", ", debugValues));

                        dataGridView1.Rows.Add(debugValues.ToArray());
                        count++; 
                    }
                    Debug.WriteLine($"Total filas agregadas: {count}");
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los productos: " + ex.Message);
                Debug.WriteLine("Error al cargar los productos: " + ex.Message);
            }
        }

        void CargarCategorias()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(Properties.Settings.Default.cc))
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT IdCategoria, NombreCategoria FROM CATEGORIA", cn);
                    dtCategorias.Clear();
                    da.Fill(dtCategorias);
                    if (dtCategorias.Rows.Count > 0)
                    {
                        cboCategoria.DataSource = dtCategorias;
                        cboCategoria.DisplayMember = "NombreCategoria";
                        cboCategoria.ValueMember = "IdCategoria";
                        cboCategoria.SelectedIndex = -1;
                    }
                    else
                    {
                        MessageBox.Show("No se encontraron categorías.");
                    }

                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las categorías: " + ex.Message);
            }
        }
        void CargarCategoriasParaCboCat()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(Properties.Settings.Default.cc))
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT IdCategoria, NombreCategoria FROM CATEGORIA", cn);
                    
                    da.Fill(dtCategorias);

                   
                    cboCat.DataSource = dtCategorias;
                    cboCat.DisplayMember = "NombreCategoria";
                    cboCat.ValueMember = "IdCategoria";
                    cboCat.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las categorías para cboCat: " + ex.Message);
            }
        }
        void CargarProveedores()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(Properties.Settings.Default.cc))
                {
                 
                    SqlDataAdapter da = new SqlDataAdapter("SELECT IdProveedor, NombreCia FROM PROVEEDOR", cn);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "proveedores");
                    cboProveedor.DataSource = ds.Tables["proveedores"];
                    cboProveedor.DisplayMember = "NombreCia"; 
                    cboProveedor.ValueMember = "IdProveedor"; 
                    cboProveedor.SelectedIndex = -1; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los proveedores: " + ex.Message);
            }
        }


        void CargarProductosPorCategoria()
        {
            try
            {
                if (cboCategoria.SelectedValue == null) return; 

                using (SqlConnection cn = new SqlConnection(Properties.Settings.Default.cc))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("pa_ProductosCategoria", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_cat", cboCategoria.SelectedValue);
                    SqlDataReader dr = cmd.ExecuteReader();
                    dataGridView1.Rows.Clear();
                    while (dr.Read())
                    {
                        dataGridView1.Rows.Add(dr.GetValue(0), dr.GetValue(1), dr.GetValue(2),
                                                dr.GetValue(3), dr.GetValue(4), dr.GetValue(5),
                                                dr.GetValue(6));
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los productos por categoría: " + ex.Message);
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            CargarCategorias(); 
            CargarProveedores();
            CargarCategoriasParaCboCat();
            CargarProductos(); 
            CargarProductosPorCategoria(); 
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            CargarProductosPorCategoria();
        }

        
        private void iconButton1_Click(object sender, EventArgs e)
        {
            CargarProductos();
        }

        //boton guardar 
        private void iconButton2_Click(object sender, EventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(txtID.Text))
            {
                MessageBox.Show("Por favor, ingrese un ID válido para el producto.");
                return;
            }

         
            string debugMessage = $"Selected Category ID: {cboCategoria.SelectedValue?.ToString() ?? "null"}\n" +
                                  $"Selected Provider ID: {cboProveedor.SelectedValue?.ToString() ?? "null"}";
            MessageBox.Show(debugMessage);

           
            var categoriasDebug = ((DataTable)cboCategoria.DataSource).AsEnumerable()
                .Select(row => $"ID: {row["IdCategoria"]}, Nombre: {row["NombreCategoria"]}")
                .ToList();
            MessageBox.Show("Categorías en DataSource: " + string.Join("; ", categoriasDebug));

          
            var proveedoresDebug = ((DataTable)cboProveedor.DataSource).AsEnumerable()
                .Select(row => $"ID: {row["IdProveedor"]}, Nombre: {row["NombreCia"]}")
                .ToList();
            MessageBox.Show("Proveedores en DataSource: " + string.Join("; ", proveedoresDebug));

            
            if (cboCategoria.SelectedValue == null || cboProveedor.SelectedValue == null)
            {
                MessageBox.Show("Por favor, asegúrese de que tanto la categoría como el proveedor estén seleccionados antes de guardar.");
                return;
            }

            
            if (!int.TryParse(cboCategoria.SelectedValue.ToString(), out int idCategoria))
            {
                MessageBox.Show("Por favor, seleccione una categoría válida.");
                return;
            }

       
            if (!CategoriaExists(idCategoria))
            {
                MessageBox.Show("El ID de la categoría no existe.");
                return;
            }

            
            try
            {
                using (SqlConnection cn = new SqlConnection(Properties.Settings.Default.cc))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("pa_ActualizarProducto", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdProducto", Convert.ToInt32(txtID.Text));
                        cmd.Parameters.AddWithValue("@NombreProducto", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@umedida", txtCantidadPorUnidad.Text);
                        cmd.Parameters.AddWithValue("@PrecioUnidad", Convert.ToDecimal(txtPrecio.Text));
                        cmd.Parameters.AddWithValue("@UnidadesEnExistencia", Convert.ToInt32(txtStock.Text));
                        cmd.Parameters.AddWithValue("@IdCategoria", idCategoria); 
                        cmd.Parameters.AddWithValue("@IdProveedor", Convert.ToInt32(cboProveedor.SelectedValue));

                        cmd.ExecuteNonQuery();
                    }
                }

                CargarProductos();
                MessageBox.Show("Producto actualizado con éxito.");
            }
            catch (SqlException sqlex)
            {
                MessageBox.Show("Error de SQL al guardar los cambios: " + sqlex.Message);
            }
            catch (FormatException fex)
            {
                MessageBox.Show("Error de formato al guardar los cambios: " + fex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar los cambios: " + ex.Message);
            }
        }

        private bool CategoriaExists(int idCategoria)
        {
            bool exists = false;
            string connectionString = Properties.Settings.Default.cc; 

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM CATEGORIA WHERE IdCategoria = @IdCategoria", cn))
                {
                    cmd.Parameters.AddWithValue("@IdCategoria", idCategoria);
                  
                    exists = (int)cmd.ExecuteScalar() > 0;
                }
            }

            return exists;
        }

       
        private void iconButton3_Click(object sender, EventArgs e)
        {
            
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];

             
                txtID.Text = row.Cells["IdProducto"].Value.ToString();
                txtNombre.Text = row.Cells["NombreProducto"].Value.ToString();
                txtCantidadPorUnidad.Text = row.Cells["umedida"].Value.ToString();
                txtPrecio.Text = row.Cells["PrecioUnidad"].Value.ToString();
                txtStock.Text = row.Cells["UnidadesEnExistencia"].Value.ToString();

                string nombreCategoria = row.Cells["NombreCategoria"].Value.ToString();
                int idCategoria;

                if (int.TryParse(row.Cells["NombreCategoria"].Value.ToString(), out idCategoria))
                {
                    
                    var categoriaRow = dtCategorias.AsEnumerable()
                        .FirstOrDefault(r => r.Field<int>("NombreCategoria") == idCategoria);

                    
                    if (categoriaRow != null)
                    {
                        cboCat.SelectedValue = categoriaRow["NombreCategoria"];
                    }
                    else
                    {
                        MessageBox.Show("La categoría seleccionada no está disponible en la lista.");
                        cboCat.SelectedIndex = -1;
                    }
                }
                else
                {
                   
                }


            }
            else
            {
               
                MessageBox.Show("Por favor seleccione una fila para editar.");
            }
        }
        // ----no tuilizo

        private void cboCat_SelectedIndexChanged(object sender, EventArgs e)
        {
           


        }

        
        private void cboCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        //agregar
        private void iconButton5_Click(object sender, EventArgs e)
        {
            if (ValidarCampos())
            {
               
                if (decimal.TryParse(txtPrecio.Text, out decimal precio) &&
                    int.TryParse(txtStock.Text, out int stock) &&
                    cboCategoria.SelectedValue != null &&
                    cboProveedor.SelectedValue != null)
                {
                    try
                    {
                        using (SqlConnection cn = new SqlConnection(Properties.Settings.Default.cc))
                        {
                            cn.Open();
                            using (SqlCommand cmd = new SqlCommand("pa_AgregarProducto", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@NombreProducto", txtNombre.Text);
                                cmd.Parameters.AddWithValue("@IdProveedor", cboProveedor.SelectedValue);
                                cmd.Parameters.AddWithValue("@IdCategoria", cboCategoria.SelectedValue);
                                cmd.Parameters.AddWithValue("@umedida", txtCantidadPorUnidad.Text);
                                cmd.Parameters.AddWithValue("@PrecioUnidad", Convert.ToDecimal(txtPrecio.Text));
                                cmd.Parameters.AddWithValue("@UnidadesEnExistencia", Convert.ToInt32(txtStock.Text));

                                
                                int result = cmd.ExecuteNonQuery();
                               
                                if (result == 1)
                                {
                                    MessageBox.Show("Producto agregado con éxito.");
                                    CargarProductos(); 
                                }
                                else
                                {
                                    MessageBox.Show("No se agregó el producto.");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al agregar el producto: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Hay errores en los datos ingresados.");
                }
            }
            else
            {
                MessageBox.Show("Por favor, complete todos los campos correctamente.");
            }
        }

        // Método de validación de los campos.
        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El campo 'Nombre' está vacío.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCantidadPorUnidad.Text))
            {
                MessageBox.Show("El campo 'Cant x Ud.' está vacío.");
                return false;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("El campo 'Precio' está vacío o no es válido. Debe ser un número mayor que 0.");
                return false;
            }

            if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
            {
                MessageBox.Show("El campo 'Stock' está vacío o no es válido. Debe ser un número no negativo.");
                return false;
            }

            if (cboCategoria.SelectedIndex < 0)
            {
                MessageBox.Show("Debe seleccionar una categoría.");
                return false;
            }

            if (cboProveedor.SelectedIndex < 0)
            {
                MessageBox.Show("Debe seleccionar un proveedor.");
                return false;
            }

           
            return true;
        }

        private void iconButton4_Click(object sender, EventArgs e)//eliminar
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idProducto = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["IdProducto"].Value);

               
                var confirmResult = MessageBox.Show("¿Estás seguro de que quieres eliminar este producto?",
                                                    "Confirmar eliminación",
                                                    MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    
                    try
                    {
                        using (SqlConnection cn = new SqlConnection(Properties.Settings.Default.cc))
                        {
                            cn.Open();
                            using (SqlCommand cmd = new SqlCommand("pa_EliminarProducto", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@IdProducto", idProducto);

                                int result = cmd.ExecuteNonQuery();

                                if (result > 0)
                                {
                                    MessageBox.Show("Producto eliminado con éxito.");
                                }
                                else
                                {
                                    MessageBox.Show("No se encontró el producto.");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al eliminar el producto: " + ex.Message);
                    }

                    CargarProductos();
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un producto para eliminar.");
            }
        }
    }
}
