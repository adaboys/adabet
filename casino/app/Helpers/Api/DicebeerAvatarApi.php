<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   DicebeerAvatarApi.php
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Helpers\Api;

use App\Helpers\Utils;

class DicebeerAvatarApi extends Api implements UserAvatarApi
{
    public const SPRITES = [
        'adventurer',
        'adventurer-neutral',
        'avataaars',
        'big-ears',
        'big-ears-neutral',
        'big-smile',
        'bottts',
        'croodles',
        'croodles-neutral',
        'female',
        'gridy',
        'human',
        'identicon',
        'initials',
        'jdenticon',
        'male',
        'micah',
        'miniavs',
        'open-peeps',
        'personas',
        'pixel-art',
        'pixel-art-neutral',
    ];

    protected function getBaseUrl(): string
    {
        return 'https://avatars.dicebear.com/api/';
    }

    protected function getScale(): int
    {
        return config('services.api.dicebeer.scale');
    }

    public function downloadAvatar()
    {
        $path = sprintf('%s/%s.svg?scale=%d', collect(self::SPRITES)->random(), Utils::generateRandomString(10), $this->getScale());
        return $this->get($path);
    }
}
