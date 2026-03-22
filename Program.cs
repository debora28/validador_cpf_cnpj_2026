using ValidadorCNPJ_Novo;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

ValidateCpfCnpj validateCpfCnpj = new ValidateCpfCnpj();

string listaDocumentosPath = "C:\\projetos\\ValidadorCNPJ_Novo\\cpfs_cnpjs.txt";

using(StreamReader reader = new StreamReader(listaDocumentosPath))
{
    string[] linhas;
    linhas = File.ReadAllLines(listaDocumentosPath);
    foreach(string linha in linhas)
    {
        Console.WriteLine(linha + " : " + validateCpfCnpj.CpfCnpjIsValid(linha).ToString());
    }
}

