using Spectre.Console;
using System;

namespace AmortizacionPrestamo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configurar la consola para mostrar caracteres especiales (moneda)
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Título de la aplicación con un diseño profesional
            AnsiConsole.Write(
                new FigletText("Amortizacion")
                    .Centered()
                    .Color(Color.Aqua));

            AnsiConsole.Write(new Rule("[yellow]Calculadora de Préstamos C# - Spectre.Console[/]").RuleStyle("grey").Centered());
            AnsiConsole.WriteLine();

            // Solicitud de datos al usuario usando Spectre.Console
            var monto = AnsiConsole.Prompt(
                new TextPrompt<decimal>("[bold white]Ingrese el monto del préstamo:[/]")
                    .PromptStyle("green")
                    .DefaultValue(10000m)
                    .ValidationErrorMessage("[red]Por favor, ingrese un monto válido.[/]"));

            var tasaAnual = AnsiConsole.Prompt(
                new TextPrompt<decimal>("[bold white]Ingrese la tasa de interés anual (%):[/]")
                    .PromptStyle("green")
                    .DefaultValue(12m)
                    .ValidationErrorMessage("[red]Por favor, ingrese una tasa válida.[/]"));

            var meses = AnsiConsole.Prompt(
                new TextPrompt<int>("[bold white]Ingrese el plazo en meses:[/]")
                    .PromptStyle("green")
                    .DefaultValue(12)
                    .ValidationErrorMessage("[red]Por favor, ingrese un número de meses válido.[/]"));

            // 1. Obtener la tasa de interés mensual (i)
            // Se divide el interés anual entre 12 meses y luego entre 100 para valor porcentual
            decimal tasaMensual = (tasaAnual / 12) / 100;

            // 2. Calcular la cuota fija mensual (C)
            // Fórmula: C = (M * i) / (1 - (1 + i)^-n)
            double i = (double)tasaMensual;
            double n = (double)meses;
            double factor = 1 - Math.Pow(1 + i, -n);
            decimal cuotaFija = (monto * tasaMensual) / (decimal)factor;
            
            // Redondeamos la cuota fija a 2 decimales
            cuotaFija = Math.Round(cuotaFija, 2);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[bold blue]Resumen del Préstamo:[/]");
            AnsiConsole.MarkupLine($"[grey]Monto:[/] [white]{monto:N2}[/]");
            AnsiConsole.MarkupLine($"[grey]Tasa Mensual:[/] [white]{tasaMensual:P3}[/]");
            AnsiConsole.MarkupLine($"[grey]Plazo:[/] [white]{meses} meses[/]");
            AnsiConsole.MarkupLine($"[bold green]Cuota Mensual Estimada: {cuotaFija:C2}[/]");
            AnsiConsole.WriteLine();

            // 3. Generar la tabla de amortización
            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.Expand();
            table.Title("[bold yellow]TABLA DE AMORTIZACIÓN[/]");
            table.Caption("[grey]Calculado con el Sistema Francés (Cuotas Fijas)[/]");

            // Definición de columnas
            table.AddColumn(new TableColumn("[blue]No. Cuota[/]").Centered());
            table.AddColumn(new TableColumn("[blue]Pago de Cuota[/]").RightAligned());
            table.AddColumn(new TableColumn("[blue]Interés a Pagar[/]").RightAligned());
            table.AddColumn(new TableColumn("[blue]Abono a Capital[/]").RightAligned());
            table.AddColumn(new TableColumn("[blue]Saldo Pendiente[/]").RightAligned());

            decimal saldoActual = monto;

            // Ciclo repetitivo for para calcular cada periodo
            for (int cuota = 1; cuota <= meses; cuota++)
            {
                // El interés a pagar es el resultado de multiplicar monto del préstamo por la tasa mensual
                decimal interesDelPeriodo = Math.Round(saldoActual * tasaMensual, 2);

                // El abono al capital es la cuota fija menos el interés a pagar
                decimal abonoCapital = Math.Round(cuotaFija - interesDelPeriodo, 2);

                // Ajuste para la última cuota (el saldo debe ser 0)
                if (cuota == meses)
                {
                    abonoCapital = saldoActual;
                    cuotaFija = abonoCapital + interesDelPeriodo;
                    saldoActual = 0;
                }
                else
                {
                    // El saldo corresponde al valor previo menos el abono al capital
                    saldoActual -= abonoCapital;
                }

                // Agregar los datos a la tabla
                table.AddRow(
                    $"[white]{cuota}[/]",
                    $"[white]{cuotaFija:N2}[/]",
                    $"[red]{interesDelPeriodo:N2}[/]",
                    $"[green]{abonoCapital:N2}[/]",
                    $"[bold cyan]{Math.Max(0, saldoActual):N2}[/]"
                );
            }

            // Mostrar la tabla final en la consola
            AnsiConsole.Write(table);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold yellow]¡Cálculo finalizado con éxito![/]");
            AnsiConsole.MarkupLine("[grey]Presione cualquier tecla para cerrar el programa...[/]");
            Console.ReadKey();
        }
    }
}
