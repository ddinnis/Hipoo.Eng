using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaEncoder.Domain
{
    public interface IMediaEncoder
    {
        bool Accept(string outputFormat);

        Task EncodeAsync(FileInfo sourceFile,FileInfo destFile,string destFormat, string[]? args, CancellationToken ct
            );
    }
}
