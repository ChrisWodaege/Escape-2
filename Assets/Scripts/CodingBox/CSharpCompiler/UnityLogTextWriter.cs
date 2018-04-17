using System.IO;
using UnityEngine;


    public class UnityLogTextWriter : TextWriter
    {
        TMPro.TextMeshProUGUI _errorMessageText;

        CodingBoxController _codingBoxController;

        public UnityLogTextWriter() : base()
        {
            _errorMessageText = GameObject.FindGameObjectWithTag("ErrorMessage").GetComponent<TMPro.TextMeshProUGUI>();
            _codingBoxController = GameObject.FindObjectOfType<CodingBoxController>();
        }

        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.ASCII; }
        }
        public override void Write(string value)
        {
            Debug.Log(value);

            _errorMessageText.text = value;

            // Hack
            if (_codingBoxController != null && value.Contains("error"))
            {
                _codingBoxController.AllowRunningCode();
            }
        }
    }
