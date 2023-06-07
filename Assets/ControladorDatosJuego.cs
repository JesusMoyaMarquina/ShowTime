using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ControladorDatosJuego : MonoBehaviour
{
    public GameObject jugador;
    public string archivoDeGuardado;
    public GameData datosJuego = new();

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
            datosJuego = JsonUtility.FromJson<GameData>(contenido);

            jugador.transform.position = datosJuego.Position;
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

        File.WriteAllText(archivoDeGuardado, cadenaJSON);

        print("Archivo guardado");
    }
}
