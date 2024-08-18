using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andrea_NameSpace
{
    public class Preference
    {
        public string ArgomentiZasm {  get; set; }
        public int BaseEP { get; set; }
        public int SizeEP {  get; set; }
        public int BaseBin {  get; set; }
        public int ResetValue { get; set; }
        public bool VerifyRam { get; set; }
        public bool salvaPreferenceOnExit { get; set; }
        public bool ResetOnLoad { get; set; }
        public bool AutoAssembler { get; set; }
        public bool AutoLoad { get; set; }
        public bool LoadAllRam { get; set; }
    }
}
