using System;
using System.Globalization;

namespace NativeVoteManagerMS.Shared.Types;

public delegate string LocalizedStringFunc(CultureInfo? culture = null);

public readonly struct LocalizedString
{
    private readonly LocalizedStringFunc _func;

    public LocalizedString(LocalizedStringFunc func) => _func = func;

    public string Resolve(CultureInfo? culture = null) => _func(culture);

    public override string ToString() => _func(null);

    public static LocalizedString From(LocalizedStringFunc func) => new(func);

    public static implicit operator LocalizedString(LocalizedStringFunc func) => new(func);
    public static implicit operator string(LocalizedString ls) => ls.ToString();
}
