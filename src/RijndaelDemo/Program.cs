using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RijndaelDemo
{
    public class Program
    {
        /// <summary>
        /// Desencripta un mensaje
        /// </summary>
        /// <param name="mensajeEncriptado">
        /// Mensaje que va a ser desencriptado
        /// </param>
        /// <param name="algoritmo">
        /// Instancia del algoritmo simétrico a usar para la desencriptación
        /// </param>
        /// <returns>
        /// Un array de bytes que representan el mensaje encriptado.
        /// </returns>
        public static byte[] Desencriptar(byte[] mensajeEncriptado, SymmetricAlgorithm algoritmo)
        {
            // ReSharper disable once NotAccessedVariable
            // La clase SymmetricAlgorithm delega el proceso de desencriptación de datos
            // Una instancia de ICryptoTransform transforma texto plano en texto cifrado o vice versa.
            // Las siguiente sentencia demuestra como crear transformaciones usando CreateDecryptor.
            var mensajeDesencriptado = new byte[mensajeEncriptado.Length];
            // Crear una ICryptoTransform que puede ser usada para desencriptar datos
            var desencriptador = algoritmo.CreateDecryptor();
            // Procedemos a descifrar el mensaje
            var memoryStream = new MemoryStream(mensajeEncriptado);
            // Creamos el CryptoStream
            var cryptoStream = new CryptoStream(memoryStream, desencriptador, CryptoStreamMode.Read);
            // Decrypting data and get the count of plain text bytes.
            cryptoStream.Read(mensajeDesencriptado, 0, mensajeDesencriptado.Length);
            // Liberamos recursos.
            memoryStream.Close();
            cryptoStream.Close();
            return mensajeDesencriptado;
        }

        /// <summary>
        /// Encripta un mensaje
        /// </summary>
        /// <param name="mensajeSinEncriptar">
        /// Mensaje que va a ser encriptado
        /// </param>
        /// <param name="algoritmo">
        /// Instancia del algoritmo simétrico a usar para la encriptación
        /// </param>
        /// <returns>
        /// Un array de bytes que representan el mensaje encriptado.
        /// </returns>
        public static byte[] Encriptar(string mensajeSinEncriptar, SymmetricAlgorithm algoritmo)
        {
            // La clase SymmetricAlgorithm delega el proceso de encriptación de datos
            // a la interfaz ICryptoTransform, la cual expone los detalles en el manejo de bloques.
            // Una instancia de ICryptoTransform transforma texto plano en texto cifrado o vice versa.
            // Las siguiente sentencia demuestra como crear transformaciones usando CreateEncryptor.
            // Crear una ICryptoTransform que puede ser usada para encriptar datos
            var encriptador = algoritmo.CreateEncryptor();
            // Las instancias de la interfaz ICryptoTransform no son útiles en si mismas.
            // .NET framework provee la clase CryptoStream para el manejo de instancias de la interfaz ICryptoTransform.
            // La clase CryptoStream actua como un envoltorio sobre un stream y transforma
            // automáticamente bloques de datos usando una interfaz ICryptoTransform.
            // La clase CryptoStream transforma datos leídos de un stream
            // (por ejemplo, desencriptando texto cifrado de un fichero)
            // o escribiendo en un stream (por ejemplo, encriptando datos generados por programa
            // y almacenando el resultado en un fichero).
            // Crear instancias de la clase CryptoStream requiere un stream real,
            // una instancia de la interfaz ICryptoTransform
            // y un valor de la enumeracion CryptoStreamMode
            // Obtenemos los bytes que representan el mensaje a encriptar
            var textoPlano = Encoding.Default.GetBytes(mensajeSinEncriptar);
            // Creamos un MemoryStream
            var memoryStream = new MemoryStream();
            // Cualquier operación de encriptación/desencriptación hara que la clase
            // que implemente el algoritmo simétrico genere una nueva clave e IV
            // si dichos valores no han sido establecidos
            // Creamos el CryptoStream
            var cryptoStream = new CryptoStream(memoryStream, encriptador, CryptoStreamMode.Write);
            // Escribimos el textoPlano hacia el CryptoStream
            cryptoStream.Write(textoPlano, 0, textoPlano.Length);
            // Terminamos la operación de encriptación.
            cryptoStream.FlushFinalBlock();
            // Liberamos.
            memoryStream.Close();
            cryptoStream.Close();
            // Obtenemos el texto cifrado del MemoryStream
            return memoryStream.ToArray();
        }

        /// <summary>
        /// Configuración del algoritmo simétrico
        /// </summary>
        /// <param name="algoritmo">
        /// Una instancia del algoritmo simétrico.
        /// </param>
        private static void ConfigurarAlgoritmo(SymmetricAlgorithm algoritmo)
        {
            // Cambiamos el valor del tamaño de bloque
            algoritmo.BlockSize = 128;
            // Establecemos el modo de cifrado y con el modo de relleno
            algoritmo.Mode = CipherMode.CBC;
            algoritmo.Padding = PaddingMode.PKCS7;
            Console.WriteLine($"Longitud de bloque: {algoritmo.BlockSize}");
            Console.WriteLine($"Modo de cifrado: {algoritmo.Mode}");
            Console.WriteLine($"Modo de relleno: {algoritmo.Padding}");
            Console.WriteLine("Pulse una tecla para continuar…\n");
            Console.ReadKey();
        }

        /// <summary>
        /// Tres formas de generar una clave.
        /// </summary>
        /// <param name="algoritmo">
        /// Una instancia del algoritmo simétrico.
        /// </param>
        private static void GenerarClave(SymmetricAlgorithm algoritmo)
        {
            // Establecemos la longitud que queremos que tenga la clave a generar.
            algoritmo.KeySize = 256;
            Console.WriteLine($"Longitud de la clave:   {algoritmo.KeySize}");
            Console.WriteLine("Pulse una tecla para continuar…\n");
            Console.ReadKey();
            // Leer sin más el valor de la clave hara que se genere.
            // sacamos la clave por consola
            Console.WriteLine("La clave: ");
            foreach (var b in algoritmo.Key)
            {
                Console.Write($"{b:X2} ");
            }
            Console.WriteLine("\nPulse una tecla para continuar…\n");
            Console.ReadKey();
            // Podemos generar otra nueva
            algoritmo.GenerateKey();
            // sacamos la nueva clave por consola
            Console.WriteLine("Otra clave: ");
            foreach (var b in algoritmo.Key)
            {
                Console.Write($"{b:X2} ");
            }
            Console.WriteLine("\nPulse una tecla para continuar…\n");
            Console.ReadKey();
            // Otra forma de crear claves sería con RNG (Random Number Generator)
            var randomNumberGenerator = RandomNumberGenerator.Create();
            // Se rellena el array de bytes de la clave con datos aleatorios
            randomNumberGenerator.GetBytes(algoritmo.Key);
            // sacamos la clave por consola
            Console.WriteLine("Otra forma de obtener una clave: ");
            foreach (var b in algoritmo.Key)
            {
                Console.Write($"{b:X2} ");
            }
            Console.WriteLine("\nPulse una tecla para continuar…\n");
            Console.ReadKey();
        }

        /// <summary>
        /// Para generar un vector de inicialización
        /// </summary>
        /// <param name="algoritmo">
        /// Una instancia del algoritmo simétrico.
        /// </param>
        private static void GenerarIv(SymmetricAlgorithm algoritmo)
        {
            // Si haces lo siguiente se genera un nuevo IV
            algoritmo.GenerateIV();
            // sacamos el IV por consola
            Console.WriteLine("IV (Vector de inicialización): ");
            foreach (var b in algoritmo.IV)
            {
                Console.Write($"{b:X2} ");
            }
            Console.WriteLine("\nPulse una tecla para continuar…\n");
            Console.ReadKey();
        }

        private static void Main()
        {
            // Este es el mensaje que vamos a encriptar.
            const string mensaje = "Programando seguridad en C#.NET";
            Console.WriteLine("Esto es el mensaje sin cifrar: " + mensaje);
            Console.WriteLine("Pulse una tecla para continuar…\n");
            Console.ReadKey();
            // Creamos el algoritmo encriptador
            var algoritmo = SymmetricAlgorithm.Create("Rijndael");
            //Se podría haber creado el algoritmo de esta otra manera:
            //RijndaelManaged algoritmoEncriptador = new RijndaelManaged();
            ConfigurarAlgoritmo(algoritmo);
            GenerarClave(algoritmo);
            GenerarIv(algoritmo);
            var mensajeEncriptado = Encriptar(mensaje, algoritmo);
            Console.WriteLine("Esto es el mensaje cifrado:");
            foreach (var b in mensajeEncriptado)
            {
                Console.Write($"{b:X2} ");
            }
            Console.WriteLine("\nPulse una tecla para continuar…\n");
            Console.ReadKey();
            var mensajeDesencriptado = Desencriptar(mensajeEncriptado, algoritmo);
            var mensajeDescrifrado = Encoding.UTF8.GetString(mensajeDesencriptado);
            Console.WriteLine("Esto es el mensaje descifrado: " + mensajeDescrifrado);
            Console.WriteLine("Pulse una tecla para terminar…\n");
            Console.ReadKey();
            algoritmo.Clear();
        }
    }
}