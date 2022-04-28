using System;

namespace DbContext
{
    public interface IProccessing
    {
        Exception Save(object obj);
        Exception Update(object obj);
        Exception Delete(object obj);
    }
}
