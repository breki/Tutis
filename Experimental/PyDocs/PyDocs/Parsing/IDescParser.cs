using PyDocs.Descriptions;

namespace PyDocs.Parsing
{
    public interface IDescParser
    {
        PackageDesc Parse(string descDir);
    }
}