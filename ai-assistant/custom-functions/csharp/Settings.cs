using System.ComponentModel.DataAnnotations;

namespace MyFirstCustomFunction;
public sealed record Settings([property: Required] string FunctionId, [property: Required] string InformationSetId, bool LoadCustomInformation);
