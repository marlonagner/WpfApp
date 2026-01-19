using System;
using System.IO;

namespace WpfApp.Services
{
    public static class Paths
    {   
        //Atributos que recebem o caminho da pasta onde ocorre a persistência em Json
        public static string DataFolder => FindDataFolder();

        public static string PedidosJson => Path.Combine(DataFolder, "pedidos.json");
        public static string PessoasJson => Path.Combine(DataFolder, "pessoas.json");
        public static string ProdutosJson => Path.Combine(DataFolder, "produtos.json");
        // Garante por segurança e encontra a pasta de persistência e que os dados vão ser persistidos.
        private static string FindDataFolder()
        {
            // Começa pela pasta onde o app está rodando
            var dir = AppDomain.CurrentDomain.BaseDirectory;

            // Sobe até 8 níveis procurando uma pasta "Data"
            for (int i = 0; i < 8 && !string.IsNullOrEmpty(dir); i++)
            {
                var candidate = Path.Combine(dir, "Data");
                if (Directory.Exists(candidate))
                    return candidate;

                var parent = Directory.GetParent(dir);
                dir = parent?.FullName;
            }

            // Se não achou, cria "Data" ao lado do executável
            var fallback = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            Directory.CreateDirectory(fallback);
            return fallback;
        }
    }
}