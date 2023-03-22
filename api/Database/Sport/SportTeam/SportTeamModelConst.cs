namespace App;

public class SportTeamModelConst {
	public enum FlagImageSource {
		Aws = 1, // Flag image is relative path.
		Betsapi = 2, // Flag image is image_id.
	}

	public static string? CalcFlagImageName(string? flag_image_name, FlagImageSource flag_image_src) {
		return flag_image_src switch {
			FlagImageSource.Betsapi => flag_image_name,
			_ => throw new AppInvalidArgumentException("Invalid image src")
		};
	}
}
