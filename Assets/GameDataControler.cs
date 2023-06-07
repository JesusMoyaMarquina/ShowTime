using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameDataControler : MonoBehaviour
{
    GameObject jugador;
    string archivoDeGuardado;
    GameData datosJuego = new();

    private void Awake()
    {
        archivoDeGuardado = Application.dataPath + "/GameData.json";

        jugador = GameObject.FindGameObjectWithTag("Player");

        CargarDatos();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CargarDatos();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            GuardarDatos();
        }
    }

    private void CargarDatos()
    {
        if (File.Exists(archivoDeGuardado))
        {
            string contenido = File.ReadAllText(archivoDeGuardado);
            datosJuego = JsonUtility.FromJson<GameData>(EncryptDecrypt(contenido));

            jugador.transform.position = datosJuego.Position;

            print("El archivo ha sido cargado");
        }
        else
            print("El archivo no existe");
    }

    private void GuardarDatos()
    {
        GameData nuevosDatos = new()
        {
            Position = jugador.transform.position
        };

        string cadenaJSON = JsonUtility.ToJson(nuevosDatos);

        File.WriteAllText(archivoDeGuardado, EncryptDecrypt(cadenaJSON));

        print("Archivo guardado");
    }

    private string keyWord = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque eget sollicitudin nisi. Nullam dignissim id orci nec condimentum. Mauris eget augue ornare, ultrices ante vitae, congue libero. In non pellentesque nibh, non pulvinar ex. Nunc sollicitudin odio id nisi congue malesuada. Aenean aliquam, dolor in pharetra cursus, arcu nisl faucibus tellus, in egestas nisl tellus quis nisi. Aliquam sed sapien sed quam ultricies auctor. Praesent sit amet rhoncus erat. Fusce mattis luctus ante at dictum. Duis lobortis ullamcorper mauris, ut suscipit libero tristique in. Nunc tempus laoreet libero at tincidunt.\r\n\r\nNam vehicula dapibus neque in ornare. Donec interdum tortor sed tincidunt feugiat. Vestibulum faucibus, nunc non bibendum rhoncus, lectus ipsum convallis enim, ut lobortis urna sem sed nunc. Nunc leo massa, suscipit a elit a, rhoncus porta massa. Etiam porttitor tellus at ornare tempus. In mi elit, interdum a pharetra sed, tincidunt ac justo. Cras a mattis tellus. Nunc sodales sagittis dui id egestas. Donec dapibus dictum tortor, consequat rhoncus ligula hendrerit ac. Nam ultrices convallis vulputate. Nunc porta scelerisque nisi, blandit tempor orci maximus a. Maecenas mauris lorem, iaculis ac vehicula eu, convallis eget dui. Nullam hendrerit quis sem et lacinia. Sed vulputate sem gravida, cursus lorem non, ultricies sem.\r\n\r\nCurabitur sodales lectus odio, sed ultricies tellus sagittis et. Aenean pulvinar in nibh vel gravida. Pellentesque at malesuada nulla. Phasellus hendrerit, neque nec sagittis imperdiet, augue risus vehicula nulla, elementum auctor orci diam eu nibh. Proin sodales dapibus augue, quis sagittis ex vulputate imperdiet. Fusce molestie massa ut libero tristique semper ut vitae risus. Aliquam ultricies magna vel porttitor hendrerit. Vestibulum eget rutrum sapien. Donec tristique elementum odio eu pretium. Pellentesque tincidunt egestas nibh ac pellentesque. Aliquam quis dui et ipsum pulvinar congue vitae at neque.\r\n\r\nNam interdum pharetra est sed pretium. Cras porttitor, purus lacinia tincidunt blandit, velit purus consequat felis, a consectetur urna nunc eget diam. Nam consequat lobortis elementum. Ut odio metus, malesuada at tempus in, egestas sed elit. Praesent rhoncus semper congue. Etiam consectetur tincidunt consequat. Duis vel ipsum nec nunc faucibus lobortis nec eget diam. Praesent porttitor libero quis egestas eleifend. Pellentesque consectetur in enim eu placerat. Donec imperdiet eros vitae elit semper, et commodo lectus posuere.\r\n\r\nSed ac maximus nisi, sed aliquet lacus. Etiam pretium neque id arcu imperdiet, non faucibus nunc mattis. Pellentesque iaculis nisi quis dictum facilisis. Aenean diam est, tristique nec nibh at, blandit gravida purus. In risus est, lobortis eu eleifend ac, euismod at nunc. Sed pretium dignissim consequat. Aenean turpis orci, vestibulum quis odio in, gravida pulvinar augue. Mauris iaculis egestas est suscipit imperdiet. Nam elementum consequat lacus id pulvinar. Curabitur vestibulum laoreet eros, id porta magna porttitor in.";
    //Xor cipher
    private string EncryptDecrypt(string Data)
    {
        string result = "";

        for (int i = 0; i < Data.Length; i++)
            result += (char)(Data[i] ^ keyWord[i % keyWord.Length]);

        return (result);
    }
}
