# PDFDoc

PDFDoc é uma biblioteca C# que facilita a geração de PDFs a partir de arquivos XML usando o iText7. Ela permite criar PDFs de maneira simples e flexível, aplicando fontes personalizadas, alinhamentos, estilos e muito mais.

## Instalação

Você pode instalar o pacote PDFDoc via NuGet:

```bash
dotnet add package PDFDoc
```
Ou, no Visual Studio, procure por PDFDoc na interface de gerenciamento de pacotes NuGet.

## Uso
### Exemplo de XML para gerar um PDF

Aqui está um exemplo de como usar o PDFDoc para gerar um PDF a partir de um arquivo XML:
```bash
<PdfDocument>
    <Text font="custom" bold="true">Título em negrito com fonte personalizada!</Text>
    <Table>
        <Row>
            <Cell alignment="center" color="red" bold="true">Header 1</Cell>
            <Cell alignment="center" color="red" bold="true">Header 2</Cell>
        </Row>
        <Row>
            <Cell>Data 1</Cell>
            <Cell>Data 2</Cell>
        </Row>
        <Row>
            <Cell>Data 3</Cell>
            <Cell>Data 4</Cell>
        </Row>
    </Table>
</PdfDocument>
```

## Exemplo de código C# usando PDFDoc

Aqui está um exemplo de como integrar a biblioteca PDFDoc ao seu projeto C#:
```bash
using iText.Kernel.Font;
using PDFDoc;

namespace ExampleApp
{
    public class Program
    {
        public static void Main()
        {
            // Exemplo de XML
            string xmlContent = @"
            <PdfDocument>
                <Text font='custom' bold='true'>Título em negrito com fonte personalizada!</Text>
                <Table>
                    <Row>
                        <Cell alignment='center' color='red' bold='true'>Header 1</Cell>
                        <Cell alignment='center' color='red' bold='true'>Header 2</Cell>
                    </Row>
                    <Row>
                        <Cell>Data 1</Cell>
                        <Cell>Data 2</Cell>
                    </Row>
                    <Row>
                        <Cell>Data 3</Cell>
                        <Cell>Data 4</Cell>
                    </Row>
                </Table>
            </PdfDocument>";

            // Instancia o PdfBuilder com o XML
            var builder = new PdfBuilder(xmlContent)
            {
                CustomFont = PdfFontFactory.CreateFont("Roboto-Medium.ttf", PdfEncodings.WINANSI, true)
            };

            // Gera o PDF como byte[] e salva em um arquivo
            byte[] pdfBytes = builder.BuildPdf();
            System.IO.File.WriteAllBytes("output.pdf", pdfBytes);

            Console.WriteLine("PDF gerado com sucesso!");
        }
    }
}
```

## Personalizações

### Você pode personalizar diversos aspectos do PDF através do XML:

-   Text: Para adicionar um título ou parágrafo com fontes e estilos.
-   Table: Para criar tabelas com cabeçalhos e linhas de dados.
-   Cell: Cada célula da tabela pode ter alinhamento, cor de fundo e texto em negrito.
-   Fontes: Defina fontes personalizadas com o atributo font="custom".

### Atributos Suportados

    font: Define a fonte usada no texto. Use custom para fontes personalizadas.
    bold: Define se o texto ou célula deve estar em negrito.
    alignment: Define o alinhamento do texto (left, center, right, justify).
    color: Define a cor de fundo da célula (em inglês ou formato hexadecimal).
    
## Licença

Este projeto está licenciado sob os termos da licença MIT. Veja o arquivo LICENSE para mais detalhes.