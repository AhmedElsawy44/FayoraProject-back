using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Services;

public interface ICodeHasher
{
    string HashCode(string code);
    bool VerifyCode(string code, string codeHash);
}
