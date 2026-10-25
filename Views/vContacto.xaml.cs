using System.Globalization;
using System.Text.RegularExpressions;

namespace djaramilloT3.Views;

public partial class vContacto : ContentPage
{
	public vContacto()
	{
		InitializeComponent();
		dpFecha.Date = DateTime.Today;
		dpFecha.MaximumDate = DateTime.Today;
	}

	// Al cambiar el tipo se ajusta el largo y el teclado del numero
	private void pkTipoId_SelectedIndexChanged(object? sender, EventArgs e)
	{
		txtNumeroId.Text = "";
		txtNumeroId.IsEnabled = pkTipoId.SelectedIndex != -1;

		switch (pkTipoId.SelectedItem?.ToString())
		{
			case "CI":
				txtNumeroId.MaxLength = 10;
				txtNumeroId.Keyboard = Keyboard.Numeric;
				txtNumeroId.Placeholder = "10 números";
				break;
			case "RUC":
				txtNumeroId.MaxLength = 13;
				txtNumeroId.Keyboard = Keyboard.Numeric;
				txtNumeroId.Placeholder = "13 números";
				break;
			case "Pasaporte":
				txtNumeroId.MaxLength = 20;
				txtNumeroId.Keyboard = Keyboard.Default;
				txtNumeroId.Placeholder = "Número de pasaporte";
				break;
			default:
				txtNumeroId.Placeholder = "Seleccione primero el tipo";
				break;
		}
	}

	// CI y RUC solo aceptan numeros; el pasaporte es libre
	private void txtNumeroId_TextChanged(object? sender, TextChangedEventArgs e)
	{
		if (pkTipoId.SelectedItem?.ToString() == "Pasaporte")
			return;

		string texto = new string((e.NewTextValue ?? "").Where(char.IsDigit).ToArray());
		if (texto != e.NewTextValue)
			txtNumeroId.Text = texto;
	}

	// Solo permite letras y espacios
	private void SoloLetras_TextChanged(object? sender, TextChangedEventArgs e)
	{
		Entry entry = (Entry)sender!;
		string texto = new string((e.NewTextValue ?? "").Where(c => char.IsLetter(c) || c == ' ').ToArray());
		if (texto != e.NewTextValue)
			entry.Text = texto;
	}

	// Solo permite numeros y un separador decimal
	private void SoloDecimales_TextChanged(object? sender, TextChangedEventArgs e)
	{
		Entry entry = (Entry)sender!;
		string texto = "";
		bool tieneSeparador = false;
		foreach (char c in e.NewTextValue ?? "")
		{
			if (char.IsDigit(c))
				texto += c;
			else if ((c == '.' || c == ',') && !tieneSeparador)
			{
				texto += '.';
				tieneSeparador = true;
			}
		}
		if (texto != e.NewTextValue)
			entry.Text = texto;
	}

	// Devuelve el mensaje del primer error encontrado, o null si todo es valido
	private string? Validar()
	{
		if (pkTipoId.SelectedIndex == -1)
			return "Seleccione el tipo de identificación";

		string tipo = pkTipoId.SelectedItem!.ToString()!;
		string numero = (txtNumeroId.Text ?? "").Trim();

		if (tipo == "CI" && !Regex.IsMatch(numero, @"^\d{10}$"))
			return "La CI debe tener 10 números";
		if (tipo == "RUC" && !Regex.IsMatch(numero, @"^\d{13}$"))
			return "El RUC debe tener 13 números";
		if (tipo == "Pasaporte" && numero == "")
			return "Ingrese el número de pasaporte";

		if (string.IsNullOrWhiteSpace(txtNombres.Text))
			return "Ingrese los nombres";
		if (string.IsNullOrWhiteSpace(txtApellidos.Text))
			return "Ingrese los apellidos";

		if (dpFecha.Date == null || dpFecha.Date > DateTime.Today)
			return "La fecha no puede ser futura";

		if (!Regex.IsMatch((txtCorreo.Text ?? "").Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
			return "Ingrese un correo válido";

		if (!double.TryParse(txtSalario.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double salario) || salario <= 0)
			return "Ingrese un salario mayor a 0";

		return null;
	}

	private async void btnGuardar_Clicked(object? sender, EventArgs e)
	{
		try
		{
			string? error = Validar();
			if (error != null)
			{
				await DisplayAlertAsync("ERROR", error, "Cerrar");
				return;
			}

			double salario = double.Parse(txtSalario.Text, CultureInfo.InvariantCulture);

			await Navigation.PushAsync(new vResumen(
				pkTipoId.SelectedItem!.ToString()!,
				txtNumeroId.Text.Trim(),
				txtNombres.Text.Trim(),
				txtApellidos.Text.Trim(),
				dpFecha.Date!.Value,
				txtCorreo.Text.Trim(),
				salario));
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("ERROR", ex.Message, "Cerrar");
		}
	}

	// Dejar formulario vacio
	private void btnLimpiar_Clicked(object? sender, EventArgs e)
	{
		pkTipoId.SelectedIndex = -1;
		txtNumeroId.Text = "";
		txtNombres.Text = "";
		txtApellidos.Text = "";
		dpFecha.Date = DateTime.Today;
		txtCorreo.Text = "";
		txtSalario.Text = "";
	}
}
