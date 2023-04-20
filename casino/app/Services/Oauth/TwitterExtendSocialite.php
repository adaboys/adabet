<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   TwitterExtendSocialite.php
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Services\Oauth;

use SocialiteProviders\Manager\SocialiteWasCalled;

class TwitterExtendSocialite
{
    public function handle(SocialiteWasCalled $socialiteWasCalled)
    {
        $socialiteWasCalled->extendSocialite('twitter', TwitterProvider::class);
    }
}
