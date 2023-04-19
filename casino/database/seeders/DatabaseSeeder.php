<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   DatabaseSeeder.php
 * 
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
*/

namespace Database\Seeders;

use App\Helpers\PackageManager;
use Illuminate\Database\Seeder;
use Illuminate\Support\Str;

class DatabaseSeeder extends Seeder
{
    
    public function run(PackageManager $packageManager)
    {
        $this->call([AssetSeeder::class]);

        
        
        foreach ($packageManager->getInstalled() as $package) {
            $seederFile = sprintf('%sPackageSeeder.php', Str::of($package->id)->title()->replace('-', ''));

            if (file_exists(base_path(sprintf('packages/%s/database/seeders/%s', $package->base_path, $seederFile)))) {
                $this->call(str_replace('.php', '', $seederFile));
            }
        }
    }
}
