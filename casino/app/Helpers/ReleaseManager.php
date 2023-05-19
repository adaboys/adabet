<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Helpers;

use GuzzleHttp\Client;
use Illuminate\Support\Facades\Cache;

class ReleaseManager
{
    
    public function getInfo($forceRefresh = FALSE)
    {
        if ($forceRefresh) {
            Cache::forget('releases');
        }

        return Cache::remember('releases', 7200, function() { 
            $client = new Client(['base_uri' => config('app.api.releases.base_url')]);
            $response = $client->request('GET', 'releases');

            return json_decode($response->getBody()->getContents());
        });
    }
}
