using System;
using MC.Modules.Keyboard;
using UnityEngine;
using UnityEngine.Events;

namespace SWL
{
    public class InputManager : MonoBehaviour
    {
        public UnityEvent<Letter> OnKeyboardInputReceived = new();

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            else if (Input.GetKeyDown(KeyCode.Q)) OnKeyboardInputReceived?.Invoke(Letter.Q);
            else if (Input.GetKeyDown(KeyCode.W)) OnKeyboardInputReceived?.Invoke(Letter.W);
            else if (Input.GetKeyDown(KeyCode.D)) OnKeyboardInputReceived?.Invoke(Letter.D);
            else if (Input.GetKeyDown(KeyCode.F)) OnKeyboardInputReceived?.Invoke(Letter.F);
            else if (Input.GetKeyDown(KeyCode.G)) OnKeyboardInputReceived?.Invoke(Letter.G);
            else if (Input.GetKeyDown(KeyCode.H)) OnKeyboardInputReceived?.Invoke(Letter.H);
            else if (Input.GetKeyDown(KeyCode.E)) OnKeyboardInputReceived?.Invoke(Letter.E);
            else if (Input.GetKeyDown(KeyCode.R)) OnKeyboardInputReceived?.Invoke(Letter.R);
            else if (Input.GetKeyDown(KeyCode.T)) OnKeyboardInputReceived?.Invoke(Letter.T);
            else if (Input.GetKeyDown(KeyCode.Y)) OnKeyboardInputReceived?.Invoke(Letter.Y);
            else if (Input.GetKeyDown(KeyCode.U)) OnKeyboardInputReceived?.Invoke(Letter.U);
            else if (Input.GetKeyDown(KeyCode.I)) OnKeyboardInputReceived?.Invoke(Letter.I);
            else if (Input.GetKeyDown(KeyCode.O)) OnKeyboardInputReceived?.Invoke(Letter.O);
            else if (Input.GetKeyDown(KeyCode.P)) OnKeyboardInputReceived?.Invoke(Letter.P);
            else if (Input.GetKeyDown(KeyCode.A)) OnKeyboardInputReceived?.Invoke(Letter.A);
            else if (Input.GetKeyDown(KeyCode.S)) OnKeyboardInputReceived?.Invoke(Letter.S);
            else if (Input.GetKeyDown(KeyCode.K)) OnKeyboardInputReceived?.Invoke(Letter.K);
            else if (Input.GetKeyDown(KeyCode.L)) OnKeyboardInputReceived?.Invoke(Letter.L);
            else if (Input.GetKeyDown(KeyCode.Z)) OnKeyboardInputReceived?.Invoke(Letter.Z);
            else if (Input.GetKeyDown(KeyCode.X)) OnKeyboardInputReceived?.Invoke(Letter.X);
            else if (Input.GetKeyDown(KeyCode.C)) OnKeyboardInputReceived?.Invoke(Letter.C);
            else if (Input.GetKeyDown(KeyCode.V)) OnKeyboardInputReceived?.Invoke(Letter.V);
            else if (Input.GetKeyDown(KeyCode.B)) OnKeyboardInputReceived?.Invoke(Letter.B);
            else if (Input.GetKeyDown(KeyCode.N)) OnKeyboardInputReceived?.Invoke(Letter.N);
            else if (Input.GetKeyDown(KeyCode.M)) OnKeyboardInputReceived?.Invoke(Letter.M);
            else if (Input.GetKeyDown(KeyCode.J)) OnKeyboardInputReceived?.Invoke(Letter.J);

            else if (Input.GetKeyDown(KeyCode.Backspace)) OnKeyboardInputReceived?.Invoke(Letter.Null); // Backspace
            else if (Input.GetKeyDown(KeyCode.Space)) OnKeyboardInputReceived?.Invoke(Letter.Space); // Space
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) OnKeyboardInputReceived?.Invoke(Letter.NewLine); // Enter

            else if (Input.GetKeyDown(KeyCode.Alpha1)) OnKeyboardInputReceived?.Invoke(Letter._1);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) OnKeyboardInputReceived?.Invoke(Letter._2);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) OnKeyboardInputReceived?.Invoke(Letter._3);
            else if (Input.GetKeyDown(KeyCode.Alpha4)) OnKeyboardInputReceived?.Invoke(Letter._4);
            else if (Input.GetKeyDown(KeyCode.Alpha5)) OnKeyboardInputReceived?.Invoke(Letter._5);
            else if (Input.GetKeyDown(KeyCode.Alpha6)) OnKeyboardInputReceived?.Invoke(Letter._6);
            else if (Input.GetKeyDown(KeyCode.Alpha7)) OnKeyboardInputReceived?.Invoke(Letter._7);
            else if (Input.GetKeyDown(KeyCode.Alpha8)) OnKeyboardInputReceived?.Invoke(Letter._8);
            else if (Input.GetKeyDown(KeyCode.Alpha9)) OnKeyboardInputReceived?.Invoke(Letter._9);
            else if (Input.GetKeyDown(KeyCode.Alpha0)) OnKeyboardInputReceived?.Invoke(Letter._0);

        }
    }
}
