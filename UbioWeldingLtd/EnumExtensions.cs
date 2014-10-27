using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UbioWeldingLtd
{

	//magic framework class from TriggerAU, still do not really unterstand it
	public static class EnumExtensions
	{


		/// <summary>
		/// returns a string of a given enum
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static String Description(this Enum e)
		{
			DescriptionAttribute[] desc = (DescriptionAttribute[])e.GetType().GetMember(e.ToString())[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
			if (desc.Length > 0)
			{
				return desc[0].Description;
			}
			else
			{
				return e.ToString();
			}
		}


		/// <summary>
		/// returns a list of strings based on the input enum
		/// </summary>
		/// <typeparam name="TEnum"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static List<String> ToEnumDescriptions<TEnum>(TEnum value) where TEnum : struct,IConvertible
		{
			List<KeyValuePair<TEnum, string>> temp = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(x => new KeyValuePair<TEnum, string>(x, ((Enum)((object)x)).Description())).ToList();
			return temp.Select(x => x.Value).ToList<String>();
		}


		/// <summary>
		/// returns an string list from an enum
		/// </summary>
		/// <typeparam name="TEnum"></typeparam>
		/// <returns></returns>
		public static List<String> ToEnumDescriptions<TEnum>() where TEnum : struct,IConvertible
		{
			return ToEnumDescriptions<TEnum>(default(TEnum)).ToList<String>();
		}
	}
}