namespace App;

using System.Text;

public class CryptoHelper {
	public static string MakeRequestSignatureInHex(string nonce) {
		return ConvertStringToHex($"Please login to verify the wallet's ownership (code: {nonce})");
	}

	public static string ConvertStringToHex(string text) {
		return Convert.ToHexString(Encoding.UTF8.GetBytes(text)).ToLower();
	}

	/// Just generate random alphabet-text for display at client side.
	public static string GenerateNonce(int length = 8) {
		var arr = new char[length];
		while (length-- > 0) {
			arr[length] = BusinessConst.alphabets[Random.Shared.Next(BusinessConst.alphabets.Length)];
		}
		return new String(arr);
	}
}
