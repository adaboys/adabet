<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Services;

class AssetService
{
    protected $api;

    public function __construct(AssetApi $api)
    {
        $this->api = $api;
    }
}
