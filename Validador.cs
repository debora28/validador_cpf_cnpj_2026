using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidadorCNPJ_Novo
{
    //public interface IValidateCpfCnpj
    //{
    //    bool CpfCnpjIsValid(string cpfCnpj);
    //}
    public class ValidateCpfCnpj
    {
        private const int CpfLength = 11;
        private const int CnpjLength = 14;

        private static readonly int[] MultipliersCpf = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2]; //Distribuição de pesos. O primeiro dígito verificador é calculado usando os 9 primeiros dígitos do CPF e começa do segundo peso da lista. O segundo dígito verificador é calculado usando os 10 primeiros dígitos do CPF e começa do primeiro peso da lista.
        private static readonly int[] MultipliersCnpj = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]; //Distribuição de pesos. O primeiro dígito verificador é calculado usando os 12 primeiros dígitos do CNPJ e começa do segundo peso da lista. O segundo dígito verificador é calculado usando os 13 primeiros dígitos do CNPJ e começa do primeiro peso da lista.

        public bool CpfCnpjIsValid(string cpfCnpj)
        {
            if (string.IsNullOrWhiteSpace(cpfCnpj))
            {
                return false;
            }
            return (CpfIsValid(cpfCnpj) || CnpjIsValid(cpfCnpj));
        }
        private static int CalculateDigits(string cnpjCpfNoMask, int length, int shift, int[] multipliers) //Shift=Deslocamento dos multiplicadores da lista de pesos a depender de qual dígito verificador será calculado.
        {                                                                                                     
            int sum = 0;

            for (int i = 0; i < length; i++)
            {
                ///Regra baseada em ASCII definida oficialmente pela Receita Federal iniciando vigência em Julho/2026. Instrução Normativa RFB nº 2229, de 15 de outubro de 2024.
                ///  https://www.gov.br/receitafederal/pt-br/centrais-de-conteudo/publicacoes/documentos-tecnicos/cnpj
                ///  https://normasinternet2.receita.fazenda.gov.br/#/consulta/externa/141102
                ///
                ///O elemento char '0' tem o valor inteiro 48 na tabela ASCII.
                ///Isso significa que ao subtrair '0' de um caractere numérico, 
                /// obtemos o valor inteiro correspondente ao dígito representado por esse caractere.
                ///Por exemplo, se cnpjCpfNoMask[i] for '5', então (int)'5' - (int)'0' resulta em 5.
                ///Do mesmo modo, considerando o novo formato de CNPJ em 2026 que pode conter letras 
                /// em sua sequência, esse mesmo recurso vai transformar 'A'..'Z' em 17..42 para realizar 
                /// os cáculos de validação que se seguirão.

                sum += (
                    (int)cnpjCpfNoMask[i] - (int)'0'
                    ) * multipliers[i + shift];
            }

            int rest = sum % 11;
            return rest == 0 || rest == 1 ? 0 : 11 - rest;
        }

        private static string RemoveMask(string cnpjCpf)
        {
            return new string(cnpjCpf.Where(c => char.IsDigit(c) || ((int)c >= (int)'A' && (int)c <= (int)'Z')).ToArray()); //Permite apenas dígitos e letras maiúsculas.
        }

        private static bool AllDigitsIdentical(string cnpjCpf)
        {
            return cnpjCpf.All(digit => digit == cnpjCpf[0]);
        }

        private static bool CpfIsValid(string cpf)
        {
            string cpfNoMask = RemoveMask(cpf);

            if (cpfNoMask.Length != CpfLength || AllDigitsIdentical(cpfNoMask))
            {
                return false;
            }

            int digit1 = CalculateDigits(cpfNoMask, cpfNoMask.Length - 2, 1, MultipliersCpf); 
            int digit2 = CalculateDigits(cpfNoMask, cpfNoMask.Length - 1, 0, MultipliersCpf); 

            return cpfNoMask.EndsWith(String.Concat(digit1, digit2));
        }

        private static bool CnpjIsValid(string cnpj)
        {
            string cnpjNoMask = RemoveMask(cnpj);

            if (cnpjNoMask.Length != CnpjLength || AllDigitsIdentical(cnpjNoMask))
            {
                return false;
            }

            int digit1 = CalculateDigits(cnpjNoMask, cnpjNoMask.Length - 2, 1, MultipliersCnpj); 
            int digit2 = CalculateDigits(cnpjNoMask, cnpjNoMask.Length - 1, 0, MultipliersCnpj); 

            return cnpjNoMask.EndsWith(String.Concat(digit1, digit2));
        }
    }
}


