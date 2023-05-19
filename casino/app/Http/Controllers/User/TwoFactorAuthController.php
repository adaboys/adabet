<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Http\Controllers\User;

use App\Http\Requests\ConfirmTwoFactorAuth;
use App\Http\Requests\DisableTwoFactorAuth;
use App\Http\Requests\VerifyTwoFactorAuth;
use App\Repositories\UserRepository;
use BaconQrCode\Renderer\Image\SvgImageBackEnd;
use BaconQrCode\Renderer\ImageRenderer;
use BaconQrCode\Renderer\RendererStyle\RendererStyle;
use BaconQrCode\Writer;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;
use Illuminate\Validation\ValidationException;
use PragmaRX\Google2FA\Google2FA;

class TwoFactorAuthController extends Controller
{
    public function enable(Request $request)
    {
        $user = $request->user();

        if ($user->totp_secret)
            throw ValidationException::withMessages([
                'email' => [__('Two-factor authentication is already enabled.')],
            ]);

        $google2fa = new Google2FA();
        $secret = $google2fa->generateSecretKey(32);
        $renderer = new ImageRenderer(
            new RendererStyle(250),
            new SvgImageBackEnd()
        );
        $qrCodeWriter = new Writer($renderer);
        $qrCodeSvg = $qrCodeWriter->writeString($google2fa->getQRCodeUrl(
            config('app.name'),
            $user->email,
            $secret
        ));

        return [
            'qr'        => $qrCodeSvg,
            'secret'    => $secret
        ];
    }

    public function confirm(ConfirmTwoFactorAuth $request)
    {
        $request->session()->put('two_factor_auth_passed', TRUE);

        
        $user = $request->user();
        $user->totp_secret = $request->secret;
        $user->save();

        return UserRepository::load($user);
    }

    public function verify(VerifyTwoFactorAuth $request)
    {
        $request->session()->put('two_factor_auth_passed', TRUE);

        return UserRepository::load($request->user());
    }

    public function disable(DisableTwoFactorAuth $request)
    {
        $request->session()->forget('two_factor_auth_passed');

        
        $user = $request->user();
        $user->totp_secret = NULL;
        $user->save();

        return UserRepository::load($user);
    }
}
