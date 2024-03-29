﻿/*
* Modified by Manuel Merino Monge <manmermon@dte.us.es>
*/
using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lslAFFSDK
{
    class CmdOptions
    {
        [Option('i', "input", Required = true, HelpText = "Input stream to be processed. Set the input to \"camera\" if you want to use the live camera stream.")]
        public string Input { get; set; }

        [Option('e', "extension", Required = false, HelpText = "Video file extension.")]
        public string Extension { get; set; }

        [Option('r', "record", Required = false, HelpText = "Record camera.")]
        public bool recordCamera { get; set; }

        [Option('d', "data", Required = true, HelpText = "Data Directory.")]
        public string DataFolder { get; set; }

        [Option('f', "numFaces", Required = false, DefaultValue = 1, HelpText = "Set Max Number of faces.")]
        public int numFaces { get; set; }

        [Option('m', "faceMode", Required = false, DefaultValue = 0, HelpText = "Set face detector mode.")]
        public int faceMode { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
