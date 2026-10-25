namespace djaramilloT3.Views;

public partial class vResumen : ContentPage
{
	// Aporte personal del afiliado al IESS: 9,45% del salario
	const double PorcentajeIess = 0.0945;

	string contenido;

	public vResumen(string tipoId, string numeroId, string nombres, string apellidos,
		DateTime fecha, string correo, double salario)
	{
		InitializeComponent();

		double aporte = Math.Round(salario * PorcentajeIess, 2);

		lblTipoId.Text = tipoId;
		lblNumeroId.Text = numeroId;
		lblNombres.Text = nombres;
		lblApellidos.Text = apellidos;
		lblFecha.Text = fecha.ToString("dd/MM/yyyy");
		lblCorreo.Text = correo;
		lblSalario.Text = "$ " + salario.ToString("F2");
		lblAporte.Text = "Aporte IESS: $ " + aporte.ToString("F2");

		contenido =
			"Identificación: " + tipoId + "\n" +
			"Número: " + numeroId + "\n" +
			"Nombres: " + nombres + "\n" +
			"Apellidos: " + apellidos + "\n" +
			"Fecha: " + lblFecha.Text + "\n" +
			"Correo: " + correo + "\n" +
			"Salario: " + lblSalario.Text + "\n" +
			"Aporte IESS (9.45%): $ " + aporte.ToString("F2") + "\n" +
			"------------------------------\n";
	}

	private async void btnExportar_Clicked(object? sender, EventArgs e)
	{
		try
		{
			// Guardar en TXT dentro de la carpeta de datos de la app
			string ruta = Path.Combine(FileSystem.AppDataDirectory, "contactos.txt");
			File.AppendAllText(ruta, contenido);

			bool compartir = await DisplayAlertAsync("Archivo exportado",
				"Datos guardados en:\n" + ruta + "\n\n¿Desea abrir/compartir el archivo?", "Sí", "No");

			if (compartir)
			{
				await Share.Default.RequestAsync(new ShareFileRequest
				{
					Title = "contactos.txt",
					File = new ShareFile(ruta)
				});
			}
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("ERROR", ex.Message, "Cerrar");
		}
	}
}
