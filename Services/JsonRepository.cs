using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace WpfApp.Services
{
    public class JsonRepository<T>
    {   
        //Atributos
        
        private readonly string _filePath;

        public JsonRepository(string filePath)
        {
            _filePath = filePath;
        }
         
        //Lista para carregamento e condicionais
        public List<T> Load()
        {
            if (!File.Exists(_filePath))
                return new List<T>();

            var json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json))
                return new List<T>();

            try
            {
                return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }
        // Método para salvar a lista de items no diretório ou criar um novo caso não exista 
        //convertendo e serializando em Json
        public void Save(List<T> items)
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);   

            var json = JsonConvert.SerializeObject(items, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}